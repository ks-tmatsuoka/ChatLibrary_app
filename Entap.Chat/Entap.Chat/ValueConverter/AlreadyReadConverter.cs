using System;
using System.Globalization;
using Xamarin.Forms;

namespace Entap.Chat
{
    public class AlreadyReadConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var isAlreadyRead = (bool)value;
            if (isAlreadyRead)
            {
                return "既読";
            }
            return "";
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isAlreadyRead = (bool)value;
            if (isAlreadyRead)
            {
                return "既読";
            }
            return "";
        }
    }
}
