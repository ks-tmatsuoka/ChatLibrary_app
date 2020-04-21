using System;
using ChatSample.Droid;
using Xamarin.Forms;

[assembly: Dependency(typeof(VideoService_Droid))]
namespace ChatSample.Droid
{
    public class VideoService_Droid : IVideoService
    {
        public bool ConvertMp4(string source, string outputPath)
        {
            // iOSで使用するメソッド
            return true;
        }
    }
}
