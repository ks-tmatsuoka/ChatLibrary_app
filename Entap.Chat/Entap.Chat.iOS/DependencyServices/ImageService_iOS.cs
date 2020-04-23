using System;
using Xamarin.Forms;
using UIKit;
using CoreGraphics;
using Foundation;
using PreserveAttribute = Xamarin.Forms.Internals.PreserveAttribute;
using Entap.Chat.iOS;

[assembly: Dependency(typeof(ImageService_iOS))]
namespace Entap.Chat.iOS
{
    [Preserve(AllMembers = true)]
    public class ImageService_iOS : IImageService
    {
        public ImageSource DownSizeImage(string filePath, double limitSize)
        {
            var bytes = GetOrientationAdjustedFilePathToByte(filePath, limitSize);
            if (bytes == null)
                return null;
            return ImageSource.FromStream(() => new System.IO.MemoryStream(bytes));
        }

        byte[] GetOrientationAdjustedFilePathToByte(string filePath, double limit)
        {
            // ios は画像の向き正しく表示されるので画像サイズだけ修正
            UIImage uiImage;
            if (filePath.Contains("http:") || filePath.Contains("https:"))
            {
                uiImage = UIImage.LoadFromData(NSData.FromUrl(new NSUrl(filePath)));
            }
            else
            {
                    uiImage = UIImage.FromFile(filePath);
            }
            if (uiImage == null)
                return null;
            int resize;
             if ((uiImage.Size.Width * uiImage.Size.Height) > limit)
            {
                //１Mピクセル超えてる
                double outArea = (double)(uiImage.Size.Width * uiImage.Size.Height) / limit;
                resize = (int)(Math.Sqrt(outArea) + 1);
            }
            else
            {
                //小さいのでそのまま
                resize = 1;
            }
            var width = (double)(uiImage.Size.Width / resize);
            var height = (double)(uiImage.Size.Height / resize);

            // リサイズ
            UIGraphics.BeginImageContext(new CoreGraphics.CGSize(width, height));
            uiImage.Draw(new CGRect(0, 0, width, height));
            UIImage resizedImage = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            // UIImage -> byte[]
            byte[] bytes = null;
            try
            {
                using (var data = resizedImage.AsPNG())
                {
                    bytes = data.ToArray();
                }
            }
            catch (Exception)
            {
                bytes = null;
            }

            return bytes;
        }
    }
}
