using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Entap.Chat
{
    [Preserve(AllMembers = true)]
    public class BoolReverseConverter : BindableObject, IValueConverter
    {
        public static BindableProperty MessageIdProperty = BindableProperty.Create("MessageId", typeof(int), typeof(BoolReverseConverter));
        public int MessageId
        {
            get => (int)GetValue(MessageIdProperty);
            set => SetValue(MessageIdProperty, value);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (MessageId == ChatListView.NotSendMessageId)
                return false;
            var val = value as bool?;
            if (val == true)
                return false;
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
