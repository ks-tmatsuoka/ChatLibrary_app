using System;
using CoreAnimation;
using Foundation;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Entap.Chat.ChatListView), typeof(Entap.Chat.iOS.ChatListViewRenderer))]
namespace Entap.Chat.iOS
{
    [Preserve(AllMembers = true)]
    public class ChatListViewRenderer : ListViewRenderer
    {
        object lastItem;
        public ChatListViewRenderer()
        {

        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);            
        }

        public override void WillMoveToWindow(UIWindow window)
        {
            base.WillMoveToWindow(window);
            var chatListView = Element as ChatListView;
            if (window is null)
                lastItem = chatListView.LastVisibleItem;
            // チャットのページでモーダルのページが表示され、そのページが閉じられると初回のみリストの表示がモーダル表示前と変わってしまう現象が発生(前のアイテムが表示されてしまっていた)
            // 対策としてページが戻る際、モーダル表示前の状態にScrollTo使い戻しておく
            if (chatListView.LastVisibleItem != null)
                chatListView.ScrollTo(lastItem, ScrollToPosition.End, false);
        }
    }
}
