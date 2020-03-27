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
                case MessageType.Image:
                    return MyImageMessageTemplate;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
