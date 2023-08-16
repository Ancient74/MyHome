using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MyHomeApp
{
    public enum IoTDeviceCapabilityType
    {
        Unknown,
        ToggleButton,
        Slider
    }

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    class NonEditableCapabilityAttribute : Attribute
    {
    }

    [Serializable]
    public class IoTDeviceCapabilityInfo
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue("")]
        public string Name { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue("")]
        public string Description { get; set; }
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue("")]
        public string Url { get; set; }
        public IoTDeviceCapabilityType Type { get; set; }
    }

    [Serializable]
    public class IoTDeviceCapability
    {
        public IoTDeviceCapability()
        {
            Info = new IoTDeviceCapabilityInfo();
        }

        public IoTDeviceCapabilityInfo Info { get; set; }

        public virtual bool IsValid() { return true; }
    }

    [Serializable]
    public class IoTDeviceToggleButtonCapability : IoTDeviceCapability
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(false)]
        public bool IsToggled { get; set; }
    }

    [Serializable]
    public class IoTDeviceSliderCapability : IoTDeviceCapability
    {
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public float Min { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(1)]
        public float Max { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Populate)]
        [DefaultValue(0)]
        public float Current { get; set; }

        public override bool IsValid() { return Max > Min; }
    }
}
