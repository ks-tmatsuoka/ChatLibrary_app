using System;
namespace Entap.Chat
{
    public class MyImageMessage : MessageBase
    {
        public MyImageMessage() : base (MessageType.MyImage)
        {
        }

        public string ImageUrl { get; set; }
        public bool IsAlreadyRead { get; set; }
    }
}
