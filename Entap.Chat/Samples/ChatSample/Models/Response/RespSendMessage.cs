using System;
using Entap.Chat;

namespace ChatSample
{
    public class RespSendMessage : ResponseBase
    {
        public SendMessageResponseBase Data { get; set; }
    }
}
