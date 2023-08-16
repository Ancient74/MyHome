using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using Xamarin.Essentials;

namespace MyHomeApp
{
    public interface IIpAddressService
    {
        IPAddress IpAddress { get; set; }
        PhysicalAddress MacAddress { get; set; }
    }

    internal class IpAddressService : IIpAddressService
    {
        private const string IP_ADDRESS_KEY = "IP_ADDRESS";
        private const string MAC_ADDRESS_KEY = "MAC_ADDRESS";
        public IPAddress IpAddress { get {
                var addressStr = Preferences.Get(IP_ADDRESS_KEY, "");
                IPAddress ipAddress;
                if (IPAddress.TryParse(addressStr, out ipAddress))
                    return ipAddress;
                return null;
            } set
            {
                Preferences.Set(IP_ADDRESS_KEY, value.ToString());
            }
        }

        public PhysicalAddress MacAddress
        {
            get
            {
                var addressStr = Preferences.Get(MAC_ADDRESS_KEY, "");
                PhysicalAddress macAddress;
                try
                {
                    macAddress = PhysicalAddress.Parse(addressStr);
                } catch
                {
                    return null;
                }
                return macAddress;
            }
            set
            {
                Preferences.Set(MAC_ADDRESS_KEY, value.ToString());
            }
        }
    }
}
