using MyHomeApp.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace MyHomeApp
{
    internal class CapabilityToViewConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SliderCapabilityViewModel)
                return SliderTemplate;
            if (value is ToggleButtonCapabilityViewModel)
                return ToggleButtonTemplate;
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        public ControlTemplate SliderTemplate { get; set; }
        public ControlTemplate ToggleButtonTemplate { get; set; }
    }
}
