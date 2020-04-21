using System;
using AVFoundation;
using CoreGraphics;
using CoreMedia;
using Entap.Chat.iOS;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(VideoService_iOS))]
namespace Entap.Chat.iOS
{
    public class VideoService_iOS : IVideoService
    {
        public ImageSource GenerateThumbImage(string url, long usecond)
        {
            NSUrl nsUrl;
            if (url.Contains("http"))
            {
                nsUrl = new Foundation.NSUrl(url);
            }
            else
            {
                nsUrl = NSUrl.FromFilename(url);
            }
            AVAssetImageGenerator imageGenerator = new AVAssetImageGenerator(AVAsset.FromUrl(nsUrl));
            imageGenerator.AppliesPreferredTrackTransform = true;
            CMTime actualTime;
            NSError error;
            CGImage cgImage = imageGenerator.CopyCGImageAtTime(new CMTime(usecond, 1000000), out actualTime, out error);
            return ImageSource.FromStream(() => new UIImage(cgImage).AsPNG().AsStream()); //mageSource.FromStream(() => new UIImage(cgImage).AsPNG().AsStream());
        }
    }
}
