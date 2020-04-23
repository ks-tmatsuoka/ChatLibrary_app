using System;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Entap.Chat
{
    [Preserve(AllMembers = true)]
    public interface IVideoService
    {
        ImageSource GenerateThumbImage(string url, long usecond=1);
    }
}
