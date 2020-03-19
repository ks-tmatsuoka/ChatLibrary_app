using System;
using Xamarin.Forms;

namespace Entap.Chat
{
    public class MessageDataTemplateSelector : DataTemplateSelector
    {
        // DataTemplates
        public DataTemplate TextMessageTemplate { get; set; }
        public DataTemplate ImageMessageTemplate { get; set; }

        public MessageDataTemplateSelector()
        {
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (!(item is MessageBase message)) throw new TypeAccessException();

            switch (message.MessageType)
            {
                case MessageType.Text:
                    return TextMessageTemplate;
                case MessageType.Image:
                    return ImageMessageTemplate;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
