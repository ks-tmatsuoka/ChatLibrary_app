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
                case 1:
                    if (message.SendUserId == Settings.Current.ChatService.GetUserId())
                        return MyTextMessageTemplate;
                    else
                        return OthersTextMessageTemplate;
                case 2:
                    if (message.SendUserId == Settings.Current.ChatService.GetUserId())
                        return MyImageMessageTemplate;
                    else
                        return OthersImageMessageTemplate;
                case 3:
                    return OthersTextMessageTemplate;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
