using System;
namespace Entap.Chat
{
    public class OthersImageMessage : MessageBase
    {
        public OthersImageMessage() : base(MessageType.OthersImage)
        {
        }


        public string ImageUrl { get; set; }
    }
}
