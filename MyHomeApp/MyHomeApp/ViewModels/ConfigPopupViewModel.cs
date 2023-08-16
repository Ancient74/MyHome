using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;

namespace MyHomeApp.ViewModels
{
    public delegate void ConfirmDelegate();
    internal class ConfigPopupViewModel : ViewModelBase
    {
        private IIpAddressService ipAddressService;
        private IDismissable dismissable;

        private string ipAddress;
        private bool isIpValid;

        private string macAddress;
        private bool isMacValid;

        public event ConfirmDelegate Confirmed;

        public string IpAddress { get => ipAddress; set
            {
                if (ipAddress == value) return;
                ipAddress = value;
                OnPropertyChanged();
            } 
        }

        public string MacAddress
        {
            get => macAddress; set
            {
                if (macAddress == value) return;
                macAddress = value;
                OnPropertyChanged();
            }
        }


        public bool IsIpValid
        {
            get => isIpValid; set
            {
                if (isIpValid == value) return;
                isIpValid = value;
                ConfirmCommand.RefreshCanExecute();
                OnPropertyChanged();
            }
        }

        public bool IsMacValid
        {
            get => isMacValid;
            set
            {
                if (isMacValid == value) return;
                isMacValid = value;
                ConfirmCommand.RefreshCanExecute();
                OnPropertyChanged();
            }
        }

        public RelayCommand ConfirmCommand { get; }

        public ConfigPopupViewModel(IIpAddressService ipAddressService, IDismissable dismissable)
        {
            this.ipAddressService = ipAddressService;
            this.dismissable = dismissable;
            ConfirmCommand = new RelayCommand(Confirm, (obj) => IsIpValid && IsMacValid);
            IpAddress = ipAddressService.IpAddress?.ToString();
            MacAddress = ipAddressService.MacAddress != null ?
                string.Join(":", ipAddressService.MacAddress.GetAddressBytes().Select(b => b.ToString("X2")))
                : "";
        }

        private void Confirm(object param)
        {
            if (!IsIpValid && IsMacValid)
                return;
            ipAddressService.IpAddress = IPAddress.Parse(IpAddress);
            ipAddressService.MacAddress = PhysicalAddress.Parse(MacAddress.ToUpper().Replace(':', '-'));
            dismissable.Dismiss();
            Confirmed?.Invoke();
        }
    }
}
