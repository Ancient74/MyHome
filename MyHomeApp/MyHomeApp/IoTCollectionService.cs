using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace MyHomeApp
{
    interface IIoTCollectionService
    {
        List<IoTDeviceModel> GetDevices();
        void SetDevices(List<IoTDeviceModel> devices);
    }

    public class IoTDeviceModel
    {
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public uint Port { get; set; }
    }

    class IoTCollectionService : IIoTCollectionService
    {
        private const string IOT_DEVICES_KEY = "IOT_DEVICES";

        public List<IoTDeviceModel> GetDevices() 
        {
            if (!Preferences.ContainsKey(IOT_DEVICES_KEY))
                return new List<IoTDeviceModel>();
            return JsonConvert.DeserializeObject<List<IoTDeviceModel>>(Preferences.Get(IOT_DEVICES_KEY, ""));
        } 
        
        public void SetDevices(List<IoTDeviceModel> devices)
        {
            var str = JsonConvert.SerializeObject(devices);
            Preferences.Set(IOT_DEVICES_KEY, str);
        }
    }
}
