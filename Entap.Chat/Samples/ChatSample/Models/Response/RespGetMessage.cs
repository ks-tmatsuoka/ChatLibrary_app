using System;
using System.Collections.Generic;

namespace ChatSample
{
    public class RespGetMessage : ResponseBase
    {
        public Contents Data { get; set; }
        public class Contents
        {
            public List<MessageList> MessageList { get; set; }
        }

        public class MessageList
        {
            public int MessageId { get; set; }
            public string SendUserId { get; set; }
            public int AlreadyReadCount { get; set; }
            public DateTime SendDateTime { get; set; }
            public int MessageType { get; set; }
            public string Text { get; set; }
            public string MediaUrl { get; set; }
        }
    }
}
