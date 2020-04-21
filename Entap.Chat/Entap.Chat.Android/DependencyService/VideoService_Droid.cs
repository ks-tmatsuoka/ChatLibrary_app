using System;
using System.Collections.Generic;
using System.IO;
using Android.Graphics;
using Android.Media;
using Entap.Chat.Android;
using Java.IO;
using Xamarin.Forms;

[assembly: Dependency(typeof(VideoService_Droid))]
namespace Entap.Chat.Android
{
    public class VideoService_Droid : IVideoService
    {
        public ImageSource GenerateThumbImage(string url, long usecond)
        {
            MediaMetadataRetriever retriever = new MediaMetadataRetriever();
            Bitmap bitmap;
            if (url.Contains("http"))
            {
                retriever.SetDataSource(url, new Dictionary<string, string>());
                bitmap = retriever.GetFrameAtTime(usecond);
            }
            else
            {
                Java.IO.File file = new Java.IO.File(url);
                var inputStream = new FileInputStream(file);
                retriever.SetDataSource(inputStream.FD);
                bitmap = retriever.GetFrameAtTime(usecond);   
            }
            if (bitmap != null)
            {
                MemoryStream stream = new MemoryStream();
                bitmap.Compress(Bitmap.CompressFormat.Png, 0, stream);
                byte[] bitmapData = stream.ToArray();
                return ImageSource.FromStream(() => new MemoryStream(bitmapData));
            }
            return null;
        }
    }
}
