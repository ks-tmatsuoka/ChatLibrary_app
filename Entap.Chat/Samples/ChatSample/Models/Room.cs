using System;
using System.Collections.Generic;

namespace ChatSample
{
    public class Room
    {
        public int RoomId { get; set; }
        public string UseId { get; set; }
        public int RoomType { get; set; }
        public string RoomName { get; set; }
        public List<string> InvitationUserIds { get; set; }
    }
}
