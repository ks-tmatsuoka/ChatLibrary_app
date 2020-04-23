using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Entap.Chat
{
    [Preserve(AllMembers = true)]
    public interface IImageService
    {
        ImageSource DownSizeImage(string file, double limitSize = 1048576.0);
    }
}
