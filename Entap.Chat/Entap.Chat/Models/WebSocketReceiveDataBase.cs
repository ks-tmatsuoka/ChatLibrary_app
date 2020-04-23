using System;
using Xamarin.Forms.Internals;

namespace Entap.Chat
{
    [Preserve(AllMembers = true)]
    public class WebSocketReceiveDataBase
    {
        public int RoomId { get; set; }
        public string Message { get; set; }
        public string AlreadyReadInfomation { get; set; }
    }
}
