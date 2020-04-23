using System;
using Xamarin.Forms.Internals;

namespace Entap.Chat
{
    [Preserve(AllMembers = true)]
    public class WebSocketReciveAlreadyReadInfomation
    {
        public int MessageId { get; set; }
        public int AlreadyReadCount { get; set; }
    }
}
