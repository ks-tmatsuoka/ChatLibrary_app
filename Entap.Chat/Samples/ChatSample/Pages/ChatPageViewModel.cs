using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ChatSample
{
    public class ChatPageViewModel : PageViewModelBase
    {
        public ChatPageViewModel(int roomId)
        {
            RoomId = roomId;
        }
        public int RoomId { get; set; }
    }
}
