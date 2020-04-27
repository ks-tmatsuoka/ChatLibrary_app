using System;
using System.Globalization;
using System.Net.Http.Headers;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Entap.Chat
{
    [Preserve(AllMembers = true)]
    public class TimeConverter : BindableObject, IValueConverter
    {
        public static BindableProperty MessageIdProperty = BindableProperty.Create("MessageId", typeof(int), typeof(TimeConverter));
        public int MessageId
        {
            get => (int)GetValue(MessageIdProperty);
            set => SetValue(MessageIdProperty, value);
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (MessageId == ChatListView.NotSendMessageId)
                return "";
            var dateTime = (DateTime)value;
            return dateTime.ToString(Settings.Current.TimeFormat);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
