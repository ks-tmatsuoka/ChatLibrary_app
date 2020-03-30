using System;
namespace Entap.Chat
{
    public class OthersTextMessage : MessageBase
    {
        public OthersTextMessage() : base(MessageType.OthersText)
        {
            UserIcon = "https://brave.entap.dev/storage/user_icon.png";
        }
        public string UserIcon { get; set; }
        public string Text { get; set; }
    }
}
