using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace SnowballMobile.Converters
{
    public class NullToTextConverter : IValueConverter
    {
        // ConverterParameter: "TextIfNotNull;TextIfNull"
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var param = (parameter as string)?.Split(';');
            if (param == null || param.Length != 2)
                return value;

            return value != null ? param[0] : param[1];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}