using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Entap.Chat
{
    [Preserve(AllMembers = true)]
    public interface IDisplayService
    {
        Thickness GetSafeArea();
        Rectangle GetDisplaySize();
    }
}
