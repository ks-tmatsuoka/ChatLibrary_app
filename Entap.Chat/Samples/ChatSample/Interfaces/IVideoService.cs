using System;
using Xamarin.Forms;

namespace ChatSample
{
    public interface IVideoService
    {
        bool ConvertMp4(string source, string outputPath);
    }
}
