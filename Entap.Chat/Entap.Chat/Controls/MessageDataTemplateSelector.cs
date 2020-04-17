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
        public DataTemplate MemberInAndOutMessageTemplate { get; set; }

        public MessageDataTemplateSelector()
        {
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (!(item is MessageBase message)) throw new TypeAccessException();

            switch (message.MessageType)
            {
                case (int)MessageType.Text:
                    if (message.SendUserId == Settings.Current.ChatService.GetUserId())
                        return MyTextMessageTemplate;
                    else
                        return OthersTextMessageTemplate;
                case (int)MessageType.Image:
                    if (message.SendUserId == Settings.Current.ChatService.GetUserId())
                        return MyImageMessageTemplate;
                    else
                        return OthersImageMessageTemplate;
                case (int)MessageType.Movie:
                    return OthersTextMessageTemplate;

                case (int)MessageType.MemberAddRoom:
                    return MemberInAndOutMessageTemplate;

                case (int)MessageType.MemberLeaveRoom:
                    return MemberInAndOutMessageTemplate;

                case (int)MessageType.Custom:
                    return OthersTextMessageTemplate;

                default:
                    throw new ArgumentException();
            }
        }
    }
}
