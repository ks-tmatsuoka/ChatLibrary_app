using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Android.Content;
using Android.Runtime;
using Android.Graphics.Drawables;

[assembly: ExportRenderer(typeof(Entap.Chat.ChatListView), typeof(Entap.Chat.Android.ChatListViewRenderer_Droid))]
namespace Entap.Chat.Android
{
    [Preserve(AllMembers = true)]
    public class ChatListViewRenderer_Droid : ListViewRenderer
    {
        public ChatListViewRenderer_Droid(Context context) : base(context)
        {
        }

        protected override void Dispose(bool disposing)
        {
            var _ChatListView = Element as ChatListView;
            _ChatListView.Dispose();
            base.Dispose(disposing);
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (Control != null && e.NewElement != null)
            {
                DisableHighlight();
                Control.Scroll += Control_Scroll;
            }
        }

        private void DisableHighlight()
        {
            if (Control is null) return;
            Control.Selector = new ColorDrawable(global::Android.Graphics.Color.Transparent);
        }

        private void Control_Scroll(object sender, global::Android.Widget.AbsListView.ScrollEventArgs e)
        {
            var _ChatListView = Element as ChatListView;

            if (_ChatListView.Messages is null || _ChatListView.Messages.Count < 1)
                return;

            
            
            var firstIndex = e.FirstVisibleItem;

            if (_ChatListView.Messages.Count >= firstIndex)
            {
                int firstVisibleIndex;
                object firstVisibleItem;
                int lastVisibleIndex = 0;
                object lastVisibleItem = new MessageBase();
                if (firstIndex == 0)
                {
                    firstVisibleIndex = firstIndex;
                    firstVisibleItem = _ChatListView.Messages[firstIndex];
                }
                else
                {
                    firstVisibleIndex = firstIndex - 1;
                    firstVisibleItem = _ChatListView.Messages[firstIndex - 1];
                }

                var lastIndex = firstIndex - 1 + e.VisibleItemCount - 1;
                for (var i = lastIndex; i >= 0; i--)
                {
                    if (_ChatListView.Messages.Count - 1 >= i)
                    {
                        lastVisibleIndex = i;
                        lastVisibleItem = _ChatListView.Messages[i];
                        break;
                    }
                }
                _ChatListView.VisibleItemUpdateForAndroid(firstVisibleIndex, firstVisibleItem, lastVisibleIndex, lastVisibleItem);
            }
            _ChatListView.OnScrolled(null, new ScrolledEventArgs(0, Control.GetChildAt(0).GetY() * -1));
        }

    }
}
