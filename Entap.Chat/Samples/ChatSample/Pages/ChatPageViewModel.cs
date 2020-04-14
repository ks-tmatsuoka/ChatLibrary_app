using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ChatSample
{
    public class ChatPageViewModel : PageViewModelBase
    {
        public ChatPageViewModel(Room room)
        {
            RoomId = room.RoomId;
            RoomType = room.RoomType;
            LastReadMessageId = room.AlreadyReadMessageId;
        }
        public int RoomId { get; set; }
        public int RoomType { get; set; }
        public int LastReadMessageId { get; set; }
    }
}
