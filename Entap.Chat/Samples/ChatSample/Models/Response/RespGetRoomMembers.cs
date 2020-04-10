using System;
using System.Collections.Generic;
using Entap.Chat;

namespace ChatSample
{
    public class RespGetRoomMembers : ResponseBase
    {
        public Contents Data { get; set; }
        public class Contents
        {
            public List<ChatMemberBase> Members { get; set; }
        }
    }
}
