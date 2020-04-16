using System;
using Xamarin.Forms;

namespace ChatSample
{
    public interface IDisplayService
    {
        Thickness GetSafeArea();
        Rectangle GetDisplaySize();
    }
}
