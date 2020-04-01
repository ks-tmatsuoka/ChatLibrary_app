using System;
namespace Entap.Chat
{
    public class MyTextMessage : MessageBase
    {
        public MyTextMessage() : base(MessageType.MyText)
        {
        }
        public string Text { get; set; }
    }
}
