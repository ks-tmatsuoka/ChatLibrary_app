using System;
using System.Collections.Generic;
using Entap.Chat;

namespace ChatSample
{
    public class RespGetMessage : ResponseBase
    {
        public Contents Data { get; set; }
        public class Contents
        {
            public List<MessageBase> MessageList { get; set; }
        }
    }
}
