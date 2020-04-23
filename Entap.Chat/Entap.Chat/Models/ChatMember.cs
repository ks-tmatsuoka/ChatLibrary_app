using System;
using Xamarin.Forms.Internals;

namespace Entap.Chat
{
    [Preserve(AllMembers = true)]
    public class ChatMemberBase
    {
        public string UserId { get; set; }
        public string UserIcon { get; set; }
        public string UserName { get; set; }
        public int RoomAdmin { get; set; }
    }
}
