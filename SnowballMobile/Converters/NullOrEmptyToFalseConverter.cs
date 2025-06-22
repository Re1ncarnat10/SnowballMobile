using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace SnowballMobile.Converters
{
    public class NullOrEmptyToFalseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is string userName && !string.Equals(userName, "Guest", StringComparison.OrdinalIgnoreCase);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}