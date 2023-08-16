using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyHomeApp.ViewModels
{

    static class CapabilityToViewModelConverter
    {
        public static CapabilityViewModelBase ConvertToViewModel(this IoTDeviceCapability model, IIoTHomeApi ioTHomeApi, IErrorHandling errorHandling)
        {
            if (model is IoTDeviceSliderCapability)
                return new SliderCapabilityViewModel(ioTHomeApi, errorHandling, model as IoTDeviceSliderCapability);
            if (model is IoTDeviceToggleButtonCapability)
                return new ToggleButtonCapabilityViewModel(ioTHomeApi, errorHandling, model as IoTDeviceToggleButtonCapability);
            return null;
        }
    }

    enum IoTDeviceViewModelStatus
    {
        NotInitialized,
        Empty,
        Normal,
        NotReachable
    }

    internal class IoTDeviceViewModel : ViewModelBase, IDisposable
    {
        private IIoTHomeApi ioTHomeApi;
        private ObservableCollection<CapabilityViewModelBase> capabilities = new ObservableCollection<CapabilityViewModelBase>();
        private bool loading = false;
        private IErrorHandling errorHandling;
        private IoTDeviceViewModelStatus status;

        public IoTDeviceViewModel(IoTDeviceModel model, IErrorHandling errorHandling)
        {
            ioTHomeApi = IoTHomeApiFactory.CreateIoTService(model);
            Status = IoTDeviceViewModelStatus.NotInitialized;
            Name = model.Name;
            this.errorHandling = errorHandling;
            ReloadCommand = new RelayCommand(obj => RefreshCapabilities());
            RefreshCapabilities();
        }

        private async void RefreshCapabilities()
        {
            Loading = true;
            bool failed = false;
            try
            {
                if (!ioTHomeApi.IsConnected())
                    await ioTHomeApi.EstablishConnection();

                var deviceCapabilities = await ioTHomeApi.GetDeviceCapabilities();
                
                Capabilities.Clear();
                foreach (var item in deviceCapabilities.Select(cap => cap.ConvertToViewModel(ioTHomeApi, errorHandling)).Where(x => x != null))
                {
                    Capabilities.Add(item);
                }
            } 
            catch (Exception)
            {
                Status = IoTDeviceViewModelStatus.NotReachable;
                Capabilities.Clear();
                failed = true;
            }
            finally
            {
                if (!failed)
                {
                    if (Capabilities.Count() == 0)
                        Status = IoTDeviceViewModelStatus.Empty;
                    else
                        Status = IoTDeviceViewModelStatus.Normal;
                }

                OnPropertyChanged(nameof(Capabilities));
                Loading = false;
            }
        }

        public void Dispose()
        {
            ioTHomeApi?.Dispose();
        }

        public RelayCommand ReloadCommand { get; }

        public string Name { get; }

        public IoTDeviceViewModelStatus Status { get => status; private set
            {
                if (status == value)
                    return;
                status = value;
                OnPropertyChanged();
            } 
        }

        public bool Loading 
        { 
            get => loading; 
            set 
            {
                if (loading == value)
                    return;
                loading = value;
                foreach(var capability in Capabilities)
                {
                    capability.IsEnabled = !loading;
                }
                OnPropertyChanged();
            } 
        }

        public ObservableCollection<CapabilityViewModelBase> Capabilities
        {
            get => capabilities;
            set
            {
                if (capabilities == value)
                    return;
                capabilities = value;
                OnPropertyChanged();
            }
        }
    }
}
