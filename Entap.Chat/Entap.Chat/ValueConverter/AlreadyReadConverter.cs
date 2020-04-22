using System;
using System.Globalization;
using Xamarin.Forms;

namespace Entap.Chat
{
    public class AlreadyReadConverter : BindableObject, IValueConverter
    {
        public static BindableProperty RoomTypeProperty = BindableProperty.Create("RoomType", typeof(int), typeof(AlreadyReadConverter));
        public int RoomType
        {
            get => (int)GetValue(RoomTypeProperty);
            set => SetValue(RoomTypeProperty, value);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var count = (int)value;
            if (count > 1)
            {
                if (RoomType == 2 || RoomType == 4)
                    return Settings.Current.AlreadyReadText + " " + (count - 1);
                return Settings.Current.AlreadyReadText;
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
