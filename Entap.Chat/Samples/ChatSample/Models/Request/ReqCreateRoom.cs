using System;
using System.Collections.Generic;

namespace ChatSample
{
    public class ReqCreateRoom
    {
        public string Data { get; set; }
    }
    public class ReqCreateRoomData
    {
        public string UserId { get; set; }
        public int RoomType { get; set; }
        public string RoomName { get; set; }
        public List<string> InvitationUserIds { get; set; }
    }
}
