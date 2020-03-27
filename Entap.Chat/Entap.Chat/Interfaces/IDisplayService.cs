using System;
using Xamarin.Forms;

namespace Entap.Chat
{
    public interface IDisplayService
    {
        Thickness GetSafeArea();
        Rectangle GetDisplaySize();
    }
}
