using System;
namespace ChatSample
{
    public class RespMessageId : ResponseBase
    {
        public Contents Data { get; set; }
        public class Contents
        {
            public string MessageId { get; set; }
        }
    }
}
