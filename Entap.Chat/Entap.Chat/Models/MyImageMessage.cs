using System;
namespace Entap.Chat
{
    public class MyImageMessage : MessageBase
    {
        public MyImageMessage() : base (MessageType.MyImage)
        {
        }


        public string ImageUrl { get; set; }
        //public string ImageUrl => $"http://placehold.jp/50x50.png?text={Id}";
    }
}
