using System;
namespace Entap.Chat
{
    public class MyTextMessage : MessageBase
    {
        public MyTextMessage() : base(MessageType.MyText)
        {
        }

        public bool IsAlreadyRead { get; set; }
        public string Text { get; set; }
    }
}
