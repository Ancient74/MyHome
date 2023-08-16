using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MyHomeApp.ViewModels
{
    public delegate void UpdateIotDeviceDelegate(IoTDeviceModel ioTDeviceModel);
    class AddIoTPopupViewModel : ViewModelBase
    {
        private IDismissable dismissable;
        private bool isIpValid = false;
        private bool isPortValid = false;
        private string name;
        private string port;
        private string ipAddress;
        private Color indicator;
        private IoTDeviceModel model;

        public event UpdateIotDeviceDelegate UpdatedIoTDevice;

        public AddIoTPopupViewModel(IDismissable dismissable, IoTDeviceModel model = null)
        {
            this.dismissable = dismissable;
            ConfirmCommand = new RelayCommand(Confirm, (obj) => Name.Length > 0 && IsIpValid && IsPortValid);
            TestConnectionCommand = new RelayCommand(TestConnection, (obj) => IsIpValid);
            this.model = model;
            if (model == null)
            {
                Name = "";
                Port = "";
                IpAddress = "";
            }
            else
            {
                Name = model.Name;
                Port = model.Port.ToString();
                IpAddress = model.IpAddress;
            }
            Indicator = ColorConverters.FromHex("#bdc3c7"); // gray
        }


        public RelayCommand ConfirmCommand { get; }
        public RelayCommand TestConnectionCommand { get; }

        public bool IsIpValid { get => isIpValid; set
            {
                if (isIpValid == value)
                    return;
                isIpValid = value;
                ConfirmCommand.RefreshCanExecute();
                TestConnectionCommand.RefreshCanExecute();
                OnPropertyChanged();
            }
        }
        public bool IsPortValid
        {
            get => isPortValid; set
            {
                if (isPortValid == value)
                    return;
                isPortValid = value;
                ConfirmCommand.RefreshCanExecute();
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => name; set
            {
                if (name == value)
                    return;
                name = value;
                ConfirmCommand.RefreshCanExecute();
                OnPropertyChanged();
            }
        }

        public string IpAddress
        {
            get => ipAddress; set
            {
                if (ipAddress == value)
                    return;
                ipAddress = value;
                OnPropertyChanged();
            }
        }

        public string Port
        {
            get => port; set
            {
                if (port == value)
                    return;
                port = value;
                OnPropertyChanged();
            }
        }

        public Color Indicator
        {
            get => indicator; set
            {
                if (indicator == value) return;
                indicator = value;
                OnPropertyChanged();
            }
        }

        public string ButtonText => model == null ? "Add device" : "Update device";
        public string TitleText => model == null ? "Add new IoT device" : "Update IoT device";

        private void Confirm(object obj)
        {
            if (!IsPortValid || !isIpValid || name.Length == 0)
                return;

            dismissable.Dismiss();
            IoTDeviceModel newModel = model ?? new IoTDeviceModel();
            newModel.Name = Name;
            newModel.IpAddress = IpAddress;
            newModel.Port = Convert.ToUInt32(Port);
            UpdatedIoTDevice?.Invoke(newModel);
        }

        private async void TestConnection(object obj)
        {
            if (!IsIpValid)
            {
                Indicator = ColorConverters.FromHex("#EA2027");
                return;
            }

            Ping ping = new Ping();
            PingReply result = await ping.SendPingAsync(IpAddress, 500);
            Indicator = result.Status == IPStatus.Success ? ColorConverters.FromHex("#2ecc71") : ColorConverters.FromHex("#EA2027"); 
        }
    }
}
