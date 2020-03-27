using System;
using System.Threading.Tasks;
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
            DisplayManager.Current.Context = context;
        }

        //protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        //{
        //    base.OnElementChanged(e);
        //    if (Control != null && e.NewElement != null)
        //    {
        //        Task.Run(async() =>
        //        {
        //            await Task.Delay(10000);
        //            //Device.BeginInvokeOnMainThread(() =>
        //            //{
        //            //    Control.ScrollTo(0, 1500);
        //            //});
        //        });
        //    }
        //}
    }
}
