using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Entap.Chat
{
    [Preserve(AllMembers = true)]
    public class AlreadyReadConverter : BindableObject, IValueConverter
    {
        public static BindableProperty IsGroupChatProperty = BindableProperty.Create("IsGroupChat", typeof(bool), typeof(AlreadyReadConverter));
        public bool IsGroupChat
        {
            get => (bool)GetValue(IsGroupChatProperty);
            set => SetValue(IsGroupChatProperty, value);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var count = (int)value;
            if (count > 1)
            {
                if (IsGroupChat)
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
