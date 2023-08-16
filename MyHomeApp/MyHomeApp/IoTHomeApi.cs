using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyHomeApp
{
    public static class IoTHomeApiFactory
    {
        public static IIoTHomeApi CreateIoTService(IoTDeviceModel model)
        {
            return new IoTHomeApi(model);
        }
    }

    public interface IIoTHomeApi : IDisposable
    {
        Task<IEnumerable<IoTDeviceCapability>> GetDeviceCapabilities();
        Task<T> PostModel<T>(T model) where T : IoTDeviceCapability;
        bool IsConnected();
        Task EstablishConnection();
    }

    internal class IoTHomeApi : IIoTHomeApi
    {
        private string ipAddress;
        private uint port;
        private TcpClient client;
        private SslStream clientStream;
        private SemaphoreSlim semaphore = new SemaphoreSlim(1);

        Dictionary<IoTDeviceCapabilityType, Type> mapping = new Dictionary<IoTDeviceCapabilityType, Type>
        {
            { IoTDeviceCapabilityType.Slider, typeof(IoTDeviceSliderCapability) },
            { IoTDeviceCapabilityType.ToggleButton, typeof(IoTDeviceToggleButtonCapability) }
        };

        private enum IoTMessage
        {
            DeviceCapability,
            DeviceCapabilities,
        }

        public IoTHomeApi(IoTDeviceModel model)
        {
            ipAddress = model.IpAddress;
            port = model.Port;
        }

        public async Task EstablishConnection()
        {
            client = new TcpClient();
            client.NoDelay = false;
            await client.ConnectAsync(ipAddress, (int)port);
            clientStream = new SslStream(client.GetStream());
            await clientStream.AuthenticateAsClientAsync("MyHome");
        }

        public bool IsConnected()
        {
            return client != null && client.Connected;
        }

        public async Task<IEnumerable<IoTDeviceCapability>> GetDeviceCapabilities()
        {
            var info = await SendMessage<List<IoTDeviceCapabilityInfo>>(IoTMessage.DeviceCapabilities.ToString());
            List<IoTDeviceCapability> capabilities = new List<IoTDeviceCapability>();
            foreach (var item in info)
            {
                JObject capabilityResponse = await SendMessage<JObject>(IoTMessage.DeviceCapability.ToString(), item.Url);
                if (!mapping.ContainsKey(item.Type))
                    continue;
                
                object deviceCapabilityObj = capabilityResponse.ToObject(mapping[item.Type]);
                if (deviceCapabilityObj != null && deviceCapabilityObj is IoTDeviceCapability)
                {
                    IoTDeviceCapability deviceCapability = deviceCapabilityObj as IoTDeviceCapability;
                    deviceCapability.Info = item;
                    if (deviceCapability.IsValid())
                        capabilities.Add(deviceCapability);
                }
            }
            return capabilities;
        }

        public async Task<T> PostModel<T>(T model) where T : IoTDeviceCapability
        {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            binaryFormatter.Serialize(stream, model);
            stream.Position = 0;
            T clone = binaryFormatter.Deserialize(stream) as T;

            var modelProperties = clone.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(prop => prop.DeclaringType != typeof(IoTDeviceCapability) 
                    && prop.GetCustomAttribute<NonEditableCapabilityAttribute>() == null)
                .ToDictionary(prop => prop.Name, prop => prop.GetValue(clone));

            var partialModel = await SendMessage<T>(clone.Info.Url, modelProperties);
            return UpdateModelPartialModel(clone, partialModel);
        }

        private T UpdateModelPartialModel<T> (T model, T partialModel) where T : IoTDeviceCapability
        {
            foreach (var prop in model.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(prop => prop.DeclaringType != typeof(IoTDeviceCapability)
                    && prop.GetCustomAttribute<NonEditableCapabilityAttribute>() == null))
            {
                prop.SetValue(model, prop.GetValue(partialModel));
            }
            return model;
        }

        private async Task<ResponseT> SendMessage<ResponseT>(string message, object data = null)
        {
            if (!IsConnected())
                throw new Exception("Not connected");

            await semaphore.WaitAsync();
            try
            {
                var writer = new StreamWriter(clientStream);
                await writer.WriteLineAsync(message);
                if (data != null)
                {
                    DefaultContractResolver contractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy(true, true)
                    };
                    var jsonString = JsonConvert.SerializeObject(
                        data,
                        new JsonSerializerSettings
                        {
                            ContractResolver = contractResolver,
                            Formatting = Formatting.None,
                            Converters = new List<JsonConverter> { new StringEnumConverter(new CamelCaseNamingStrategy(true, true), true) }
                        });
                    await writer.WriteLineAsync(jsonString);
                }
                await writer.FlushAsync();
                var reader = new StreamReader(clientStream);
                var messageResponse = await reader.ReadLineAsync();

                if (messageResponse.ToLower().StartsWith("error"))
                    throw new Exception(messageResponse.Substring(5));

                string headerResponse = messageResponse.Substring(0, message.Length);
                string dataResponse = messageResponse.Substring(message.Length);
                if (headerResponse != message)
                    throw new ArgumentException("Unexpected response from server", headerResponse);

                return JsonConvert.DeserializeObject<ResponseT>(dataResponse);
            }
            finally
            {
                semaphore.Release();
            }
        }
        public void Dispose()
        {
            clientStream.Dispose();
            client.Dispose();
        }
    }
}
