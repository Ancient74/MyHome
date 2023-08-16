using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace MyHomeApp
{
    public class ColorToSolidColorBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is Color color ? new SolidColorBrush(color) : null;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
            value is SolidColorBrush brush ? brush.Color : Color.Transparent;
    }
}
