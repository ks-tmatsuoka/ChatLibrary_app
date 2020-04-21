using System;
using System.Threading.Tasks;
using AVFoundation;
using ChatSample.iOS;
using Foundation;
using Xamarin.Forms;

[assembly: Dependency(typeof(VideoService_iOS))]
namespace ChatSample.iOS
{
    public class VideoService_iOS : IVideoService
    {
        public bool ConvertMp4(string source, string outputPath)
        {
            var comp = new TaskCompletionSource<bool>();
            Task.Run(() =>
            {
                var asset = AVAsset.FromUrl(NSUrl.FromFilename(source));
                AVAssetExportSession export = new AVAssetExportSession(asset, AVAssetExportSession.PresetLowQuality);
                export.OutputUrl = NSUrl.FromFilename(outputPath);
                export.OutputFileType = AVFileType.Mpeg4;
                export.ShouldOptimizeForNetworkUse = true;
                export.ExportAsynchronously(() =>
                {
                    if (export.Error != null)
                    {
                        System.Diagnostics.Debug.WriteLine("ConvertMp4 error:" + export.Error.LocalizedDescription);
                        comp.SetResult(false);
                        return;
                    }
                    comp.SetResult(true);
                    //MessagingCenter.Send<CaptureController>(this, "ConvertMp4");
                });
            });
            return comp.Task.Result;
        }
    }
}
