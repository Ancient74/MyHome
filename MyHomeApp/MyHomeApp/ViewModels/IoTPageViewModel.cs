using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;

namespace MyHomeApp.ViewModels
{
    public class IoTPageViewModel : ViewModelBase
    {
        private IMyHomeApi myHomeApi;
        private IIoTPageNavigator ioTPageNavigator;
        private IErrorHandling errorHandling;
        private IReturnToMainPageNavigator returnToMainPageNavigator;
        private IoTCollectionService ioTCollectionService;
        private ObservableCollection<IoTDeviceModel> devices;
        private IUserPrompt userPrompt;

        public interface IIoTPageNavigator
        {
            void NavigateToAddIotPage(IoTDeviceModel model = null);
            Task NavigateToIotDevicePage(IoTDeviceModel model);
        }

        public interface IUserPrompt
        {
            Task<bool> AskUser(string question);
        }

        public IoTPageViewModel(IMyHomeApi myHomeApi, IIoTPageNavigator ioTPageNavigator, IErrorHandling errorHandling, IReturnToMainPageNavigator returnToMainPageNavigator, IUserPrompt userPrompt)
        {
            this.myHomeApi = myHomeApi;
            this.ioTPageNavigator = ioTPageNavigator;
            this.errorHandling = errorHandling;
            this.returnToMainPageNavigator = returnToMainPageNavigator;
            this.userPrompt = userPrompt;

            this.ioTCollectionService = new IoTCollectionService();

            Devices = new ObservableCollection<IoTDeviceModel>(ioTCollectionService.GetDevices());

            AddIoTDeviceCommand = new RelayCommand(AddIoTDevice);
            EditIoTDeviceCommand = new RelayCommand(EditIoTDevice);
            DeleteIoTDeviceCommand = new RelayCommand(DeleteIoTDevice);
            OpenIoTDeviceCommand = new RelayCommand(OpenIoTDevice);
        }

        public RelayCommand AddIoTDeviceCommand { get; }
        public RelayCommand EditIoTDeviceCommand { get; }
        public RelayCommand DeleteIoTDeviceCommand { get; }
        public RelayCommand OpenIoTDeviceCommand { get; }

        public ObservableCollection<IoTDeviceModel> Devices { get => devices; set
            {
                if (devices == value)
                    return;
                devices = value;
                OnPropertyChanged();
            }
        }

        private void AddIoTDevice(object obj)
        {
            ioTPageNavigator.NavigateToAddIotPage();
        }

        private void EditIoTDevice(object model)
        {
            ioTPageNavigator.NavigateToAddIotPage((IoTDeviceModel)model);
        }

        public void DeviceUpdated(IoTDeviceModel device)
        {
            if (!Devices.Contains(device))
                Devices.Add(device);
            Devices = new ObservableCollection<IoTDeviceModel>(Devices);
            ioTCollectionService.SetDevices(Devices.ToList());
        }

        private async void DeleteIoTDevice(object obj)
        {
            var model = (IoTDeviceModel)obj;
            if (await userPrompt.AskUser($"Do you really want to delete {model.Name}?"))
            {
                if (Devices.Remove(model))
                {
                    Devices = new ObservableCollection<IoTDeviceModel>(Devices);
                    ioTCollectionService.SetDevices(Devices.ToList());
                }
            }
        }
        
        private void OpenIoTDevice(object obj)
        {
            var model = (IoTDeviceModel)obj;
            ioTPageNavigator.NavigateToIotDevicePage(model);
        }
    }
}
