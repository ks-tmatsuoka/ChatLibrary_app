using System;
using Xamarin.Forms;

namespace Entap.Chat
{
    public class MessageDataTemplateSelector : DataTemplateSelector
    {
        // DataTemplates
        public DataTemplate OthersTextMessageTemplate { get; set; }
        public DataTemplate MyTextMessageTemplate { get; set; }
        public DataTemplate MyImageMessageTemplate { get; set; }
        public DataTemplate OthersImageMessageTemplate { get; set; }

        public MessageDataTemplateSelector()
        {
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (!(item is MessageBase message)) throw new TypeAccessException();

            switch (message.MessageType)
            {
                case MessageType.OthersText:
                    return OthersTextMessageTemplate;
                case MessageType.MyText:
                    return MyTextMessageTemplate;
                case MessageType.MyImage:
                    return MyImageMessageTemplate;
                case MessageType.OthersImage:
                    return OthersImageMessageTemplate;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
