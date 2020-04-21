using System;
using System.Globalization;
using Xamarin.Forms;

namespace Entap.Chat
{
    public class MemberInAndOutMessageConverter : BindableObject, IValueConverter
    {
        public static BindableProperty MessageTypeProperty = BindableProperty.Create("MessageType", typeof(int), typeof(MemberInAndOutMessageConverter));
        public int MessageType
        {
            get => (int)GetValue(MessageTypeProperty);
            set => SetValue(MessageTypeProperty, value);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = value as string;
            if (MessageType == (int)Entap.Chat.MessageType.MemberAddRoom)
            {
                val += Settings.Current.MemberAddRoomText;
            }
            else if (MessageType == (int)Entap.Chat.MessageType.MemberLeaveRoom)
            {
                val += Settings.Current.MemberLeaveRoomText;
            }
            return val;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
