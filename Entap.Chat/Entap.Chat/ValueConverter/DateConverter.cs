using System;
using System.Globalization;
using Xamarin.Forms;

namespace Entap.Chat
{
    public class DateConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = (DateTime)value;
            if (dateTime.ToString("yyyy/MM/dd") == DateTime.Now.ToString("yyyy/MM/dd"))
                return "今日";
            return dateTime.ToString("MM/dd");
        }
        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var dateTime = (DateTime)value;
            if (dateTime.ToString("yyyy/MM/dd") == DateTime.Now.ToString("yyyy/MM/dd"))
                return "今日";
            return dateTime.ToString("MM/dd");
        }
    }
}
