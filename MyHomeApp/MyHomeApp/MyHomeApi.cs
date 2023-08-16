using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;

namespace MyHomeApp
{
    public enum MonitorMode
    {
        PCScreenOnly = 1, SecondScreenOnly, Extend, Duplicate
    }

    public enum ApplicationStatus
    {
        Initializing, IpIsNotSet, Unreachable, Normal
    }

    public enum ShutdownMode 
    { 
        NormalShutdown = 1, ForceShutdown, NormalRestart, ForceRestart
    }

    public enum AudioDeviceType
    {
        Input, Output
    }

    public class AudioDeviceModel
    {
        public AudioDeviceModel() : this("", "", 0.0f, false) { }
        public AudioDeviceModel(string name, string id, float volumeLevel, bool isMuted)
        {
            Name = name;
            Id = id;
            VolumeLevel = volumeLevel;
            IsMuted = isMuted;
        }

        public string Name { get; set; }
        public string Id { get; set; }
        public float VolumeLevel { get; set; }
        public bool IsMuted { get; set; }
    }

    public interface IMyHomeApi
    {
        Task SetMonitorMode(MonitorMode monitorMode);
        Task OpenBigPicture();
        Task OpenInBrowser(string url);
        Task<ApplicationStatus> Ping();
        Task PCShutdown(ShutdownMode shutdownMode);
        Task<AudioDeviceModel> GetActiveAudioDevice(AudioDeviceType type);
        Task<AudioDeviceModel> UpdateDevice(AudioDeviceType type, AudioDeviceModel deviceModel);
        Task ActivateAudioDevice(AudioDeviceType type, string id);
        Task<List<AudioDeviceModel>> GetAllAudioDevices(AudioDeviceType type);
        Task WakeOnLan();
    }

    public class MyHomeApi : IMyHomeApi
    {
        private HttpClient client;
        private IIpAddressService ipAddressService;

        private const string SCHEME = "https";
        private const int PORT = 42069;

        public MyHomeApi(IIpAddressService ipAddressService)
        {
            this.ipAddressService = ipAddressService;
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(3);
        }

        public async Task ActivateAudioDevice(AudioDeviceType type, string id)
        {
            await SendRequest("api/Audio/ActivateAudioDevice/" + type.ToString(), HttpMethod.Post, id);
        }

        public async Task<List<AudioDeviceModel>> GetAllAudioDevices(AudioDeviceType type)
        {
            var response = await SendRequest("api/Audio/GetAllAudioDevices/" + type.ToString(), HttpMethod.Get, null);
            string responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<AudioDeviceModel>>(responseString);
        }

        public async Task<AudioDeviceModel> GetActiveAudioDevice(AudioDeviceType type)
        {
            var response = await SendRequest("api/Audio/GetActiveAudioDevice/" + type.ToString(), HttpMethod.Get, null);
            string responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<AudioDeviceModel>(responseString);
        }

        public async Task<AudioDeviceModel> UpdateDevice(AudioDeviceType type, AudioDeviceModel deviceModel)
        {
            var response = await SendRequest("api/Audio/UpdateDevice/" + type.ToString(), HttpMethod.Put, deviceModel);
            string responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<AudioDeviceModel>(responseString);
        }

        public async Task SetMonitorMode(MonitorMode monitorMode)
        {
            await SendRequest("api/Monitor/MonitorMode", HttpMethod.Post, monitorMode);
        }

        public async Task OpenInBrowser(string url)
        {
            await SendRequest("api/Desktop/OpenInBrowser", HttpMethod.Post, url);
        }

        public async Task OpenBigPicture()
        {
            await SendRequest("api/Monitor/OpenBigPicture", HttpMethod.Post, null);
        }

        public async Task PCShutdown(ShutdownMode shutdownMode)
        {
            await SendRequest("api/Desktop/PCShutdown", HttpMethod.Post, shutdownMode);
        }

        public async Task<ApplicationStatus> Ping()
        {
            if (ipAddressService.IpAddress == null)
                return ApplicationStatus.IpIsNotSet;

            try
            {
                await SendRequest("api/Desktop/Ping", HttpMethod.Get, null);
                return ApplicationStatus.Normal;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<HttpResponseMessage> SendRequest(string path, HttpMethod method, object data)
        {
            if (ipAddressService.IpAddress == null)
            {
                throw new ArgumentException("Ip address is not set");
            }
            var builder = new UriBuilder();
            builder.Host = ipAddressService.IpAddress.ToString();
            builder.Scheme = SCHEME;
            builder.Port = PORT;
            builder.Path = path;

            var uri = builder.Uri;
            var message = new HttpRequestMessage();
            message.Method = method;
            if (data != null)
            {
                string jsonString = JsonConvert.SerializeObject(
                    data,
                    new Newtonsoft.Json.Converters.StringEnumConverter(new CamelCaseNamingStrategy(true, false), true));
                message.Content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            }
            message.RequestUri = uri;
            var response = await client.SendAsync(message);
            response.EnsureSuccessStatusCode();
            return response;
        }

        public Task WakeOnLan()
        {
            if (ipAddressService.MacAddress == null)
            {
                throw new ArgumentException("MAC address is not set");
            }

            var udp = new UdpClient();
            udp.EnableBroadcast = true;
            udp.Connect("255.255.255.255", 0);
            byte[] mac = ipAddressService.MacAddress.GetAddressBytes();
            
            const uint sixBytes = 6;
            const uint repetitions = 16;
            byte[] wakeOnLanPacket = new byte[sixBytes + repetitions * mac.Length];
            for (int i = 0; i < sixBytes; i++)
            {
                wakeOnLanPacket[i] = 0xFF;
            }
            for (int i = 0; i < repetitions; i++)
            {
                for (int j = 0; j < mac.Length; j++)
                {
                    wakeOnLanPacket[sixBytes + (i * mac.Length) + j] = mac[j];
                }
            }

            return udp.SendAsync(wakeOnLanPacket, wakeOnLanPacket.Length);
        }
    }
}
