using System;
using Entap.Chat.Android;
using Xamarin.Forms;

[assembly: Dependency(typeof(DisplayService_Droid))]
namespace Entap.Chat.Android
{
    public class DisplayService_Droid : IDisplayService
    {
        public Thickness GetSafeArea()
        {
            return new Thickness(0, 0, 0, 0);
        }
    }
}
