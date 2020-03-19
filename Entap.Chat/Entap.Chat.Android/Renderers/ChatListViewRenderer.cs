using System;
using Android.Content;
using Android.Runtime;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(Entap.Chat.ChatListView), typeof(Entap.Chat.Android.ChatListViewRenderer))]
namespace Entap.Chat.Android
{
    [Preserve(AllMembers = true)]
    public class ChatListViewRenderer : ListViewRenderer
    {
        public ChatListViewRenderer(Context context) : base(context)
        {
        }
    }
}
