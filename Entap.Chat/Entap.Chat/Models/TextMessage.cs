using System;
namespace Entap.Chat
{
    public class TextMessage : MessageBase
    {
        public TextMessage() : base(MessageType.Text)
        {
        }

        public string Text { get; set; }
        //public string Text => Id.ToString();
    }
}
