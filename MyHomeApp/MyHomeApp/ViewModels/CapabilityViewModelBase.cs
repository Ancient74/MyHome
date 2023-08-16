using System;
using System.Collections.Generic;
using System.Text;

namespace MyHomeApp.ViewModels
{
    internal abstract class CapabilityViewModelBase : ViewModelBase
    {
        public CapabilityViewModelBase(IoTDeviceCapability model)
        {
            Name = model.Info.Name;
            Description = model.Info.Description;
        }

        public abstract bool IsEnabled { get; set; }
        public string Name { get; }
        public string Description { get; }
    }
}
