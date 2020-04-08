using System;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ChatSample
{
    public class ImageManager
    {
        public ImageManager()
        {
        }

        public static async Task ImageShare(string imagePath)
        {
            var mediaFolderPath = DependencyService.Get<IFileService>().GetMediaFolderPath();
            var extension = System.IO.Path.GetExtension(imagePath);
            string filePath = mediaFolderPath;
            if (extension.ToLower() == ".jpeg" || extension.ToLower() == ".jpg")
            {
                filePath += "/temp.jpeg";
            }
            else if (extension.ToLower() == ".png")
            {
                filePath += "/temp.png";
            }
            else if (extension.ToLower() == ".pdf")
            {
                filePath += "/temp.pdf";
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("エラー", "こちらのファイルは表示できません", "閉じる");
                return;
            }

            bool result;
            if (imagePath.Contains("http://") || imagePath.Contains("https://"))
            {
                result = await ImageManager.DownloadWebImageFile(imagePath, filePath);
            }
            else
            {
                result = FileManager.FileCopy(imagePath, filePath);
            }

            if (!result)
            {
                await Application.Current.MainPage.DisplayAlert("エラー", "ファイルが取得できませんでした", "閉じる");
                return;
            }

            string str = "error";
            DependencyService.Get<IFileService>().OpenShareMenu(filePath, ref str);
        }

        public static Task<bool> DownloadWebImageFile(string dlImagPath, string savePath)
        {
            var cmp = new TaskCompletionSource<bool>();
            bool downloadWebImageFileCmp = false;
            try
            {
                WebClient client = new WebClient();
                client.DownloadFileAsync(new Uri(dlImagPath), savePath);
                client.DownloadFileCompleted += (object sender, System.ComponentModel.AsyncCompletedEventArgs e) =>
                {
                    try
                    {
                        cmp.SetResult(true);
                    }
                    catch (System.Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("DownloadFileCompleted Error: " + ex);
                    }
                    downloadWebImageFileCmp = true;
                };

                Task.Run(async () =>
                {
                    await Task.Delay(30000);
                    if (downloadWebImageFileCmp)
                        return;
                    client.CancelAsync();
                    cmp.SetResult(false);
                });
            }
            catch (Exception ex)
            {
                cmp.SetResult(false);
                System.Diagnostics.Debug.WriteLine("FileDownLoadError : " + ex);
            }
            return cmp.Task;
        }
    }
}
