using System;
namespace Entap.Chat
{
    public class OthersImageMessage : MessageBase
    {
        public OthersImageMessage() : base(MessageType.OthersImage)
        {
            UserIcon = "https://brave.entap.dev/storage/user_icon.png";
        }
        public string UserIcon { get; set; }
        public string ImageUrl { get; set; }
    }
}
