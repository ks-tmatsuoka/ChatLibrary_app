using System;
using Android.Content;
using Entap.Chat;
using Entap.Chat.Android;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(CustomContentView), typeof(CustomContentViewRenderer_Droid))]
namespace Entap.Chat.Android
{
    public class CustomContentViewRenderer_Droid : ViewRenderer
    {
        public CustomContentViewRenderer_Droid(Context context) : base(context)
        {
        }
    }
}
