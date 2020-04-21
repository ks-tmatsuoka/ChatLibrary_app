using System;
using Xamarin.Forms;

namespace Entap.Chat
{
    public interface IVideoService
    {
        ImageSource GenerateThumbImage(string url, long usecond=1);
    }
}
