using System;
using System.Collections.Generic;

namespace ChatSample
{
    public class RespGetRoomList : ResponseBase
    {
        public Contents Data { get; set; }
        public class Contents
        {
            public List<Room> Rooms { get; set; }
        }
    }
}
