using System;
using Foundation;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Entap.Chat.ChatListView), typeof(Entap.Chat.iOS.ChatListViewRenderer))]
namespace Entap.Chat.iOS
{
    [Preserve(AllMembers = true)]
    public class ChatListViewRenderer : CollectionViewRenderer
    {
    }
}
