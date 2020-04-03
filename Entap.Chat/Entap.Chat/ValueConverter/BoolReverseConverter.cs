using System;
using System.Globalization;
using Xamarin.Forms;

namespace Entap.Chat
{
    public class BoolReverseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as bool?;
            if (val == true)
                return false;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as bool?;
            if (val == true)
                return false;
            return true;
        }
    }
}
