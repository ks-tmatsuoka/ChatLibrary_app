using System;
using System.Globalization;
using System.Net.Http.Headers;
using Xamarin.Forms;

namespace Entap.Chat
{
    public class TimeConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {

            var dateTime = (DateTime)value;
            return dateTime.ToString("hh:mm");
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
