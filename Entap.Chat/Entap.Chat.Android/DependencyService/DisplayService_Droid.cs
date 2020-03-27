using System;
using Android.Content;
using Entap.Chat.Android;
using Xamarin.Forms;

[assembly: Dependency(typeof(DisplayService_Droid))]
namespace Entap.Chat.Android
{
    public class DisplayService_Droid : IDisplayService
    {
        public Rectangle GetDisplaySize()
        {
            var rect = DisplayManager.Current.GetDisplaySize();
            if (rect.Width > 0)
                return new Rectangle(0, 0, rect.Width, rect.Height);
            return new Rectangle(0, 0, 100, 100);
        }

        public Thickness GetSafeArea()
        {
            return new Thickness(0, 0, 0, 0);
        }
    }
}
