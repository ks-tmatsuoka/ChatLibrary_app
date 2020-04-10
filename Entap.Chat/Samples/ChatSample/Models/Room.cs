using System;
using System.Collections.Generic;

namespace ChatSample
{
    public class Room
    {
        public int RoomId { get; set; }
        public int RoomType { get; set; }
        public string RoomName { get; set; }
        public int AlreadyReadMessageId { get; set; }
        public int LastMessageId { get; set; }
    }
}
