using System;
using System.Collections.Generic;

namespace ChatSample
{
    public class ReqAddRoommember
    {
        public string Data { get; set; }
    }
    public class ReqAddRoommemberData
    {
        public string UserId { get; set; }
        public int RoomId { get; set; }
        public List<string> InvitationUserIds { get; set; }
    }
}
