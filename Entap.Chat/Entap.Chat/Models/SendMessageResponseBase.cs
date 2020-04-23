using System;
using Xamarin.Forms.Internals;

namespace Entap.Chat
{
    [Preserve(AllMembers = true)]
    public class SendMessageResponseBase
    {
        public int MessageId { get; set; }
        public DateTime SendDateTime { get; set; }
    }
}
