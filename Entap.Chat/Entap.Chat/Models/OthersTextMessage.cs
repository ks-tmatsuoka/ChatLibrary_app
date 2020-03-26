using System;
namespace Entap.Chat
{
    public class OthersTextMessage : MessageBase
    {
        public OthersTextMessage() : base(MessageType.OthersText)
        {
        }

        public string Text { get; set; }
    }
}
