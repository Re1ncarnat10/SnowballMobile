using System;
using System.Globalization;
namespace SnowballMobile.Converters
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool booleanValue)
            {
                return !booleanValue;
            }

            // Return a default value or handle invalid input gracefully
            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool booleanValue)
            {
                return !booleanValue;
            }

            // Return a default value or handle invalid input gracefully
            return Binding.DoNothing;
        }
    }
}