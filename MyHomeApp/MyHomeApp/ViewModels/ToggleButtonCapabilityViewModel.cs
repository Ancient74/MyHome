using System;
using System.Collections.Generic;
using System.Text;

namespace MyHomeApp.ViewModels
{
    internal class ToggleButtonCapabilityViewModel : CapabilityViewModelBase
    {
        private IIoTHomeApi ioTHomeApi;
        private IErrorHandling errorHandling;
        private IoTDeviceToggleButtonCapability model;
        private bool isEnabled = true;
        private bool isToggled;

        public ToggleButtonCapabilityViewModel(IIoTHomeApi ioTHomeApi, IErrorHandling errorHandling, IoTDeviceToggleButtonCapability model) : base(model)
        {
            this.ioTHomeApi = ioTHomeApi;
            this.errorHandling = errorHandling;
            this.model = model;

            isToggled = model.IsToggled;
        }

        public bool IsToggled { get => isToggled; set 
            {
                if (IsToggled == value)
                    return;
                isToggled = value;
                OnPropertyChanged();
                UpdateModel();
            }
        }

        public override bool IsEnabled
        {
            get => isEnabled; set
            {
                if (isEnabled == value)
                    return;
                isEnabled = value;
                OnPropertyChanged();
            }
        }

        private async void UpdateModel()
        {
            bool fallbackValue = model.IsToggled;
            try
            {
                model.IsToggled = IsToggled;
                model = await ioTHomeApi.PostModel(model);
            } catch (Exception e)
            {
                errorHandling.ShowErrorMessage(e.Message);
                model.IsToggled = isToggled = fallbackValue;
            }
            finally
            {
                OnPropertyChanged(nameof(IsToggled));
            }
        }
    }
}
