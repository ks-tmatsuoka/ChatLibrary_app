using System;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ChatSample
{
    public class MediaManager
    {
        public MediaManager()
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
                result = await DownloadWebFile(imagePath, filePath);
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

        public static Task<bool> DownloadWebFile(string dlPath, string savePath)
        {
            var cmp = new TaskCompletionSource<bool>();
            bool downloadWebImageFileCmp = false;
            try
            {
                WebClient client = new WebClient();
                client.DownloadFileAsync(new Uri(dlPath), savePath);
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

        public static async Task ImageDownload(string imageUrl)
        {
            var dlFolderPath = DependencyService.Get<IFileService>().GetDownloadFolderPath();
            var extension = System.IO.Path.GetExtension(imageUrl);
            string filePath = dlFolderPath;
            if (extension.ToLower() == ".jpeg" || extension.ToLower() == ".jpg")
            {
                filePath += "/" + Guid.NewGuid() + ".jpeg";
            }
            else if (extension.ToLower() == ".pdf")
            {
                filePath += "/" + Guid.NewGuid() + ".pdf";
            }
            else
            {
                filePath += "/" + Guid.NewGuid() + ".png";
            }
            bool? dlResult;
            if (Device.RuntimePlatform == Device.Android)
            {
                dlResult = await DownloadWebFile(imageUrl, filePath);
            }
            else
            {
                if (imageUrl.Contains("http://") || imageUrl.Contains("https://"))
                {
                    var mediaFolderPath = DependencyService.Get<IFileService>().GetMediaFolderPath();
                    if (extension.ToLower() == ".jpeg" || extension.ToLower() == ".jpg")
                    {
                        mediaFolderPath += "/" + Guid.NewGuid() + ".jpeg";
                    }
                    else if (extension.ToLower() == ".pdf")
                    {
                        mediaFolderPath += "/" + Guid.NewGuid() + ".pdf";
                    }
                    else
                    {
                        mediaFolderPath += "/" + Guid.NewGuid() + ".png";
                    }
                    var result = await DownloadWebFile(imageUrl, mediaFolderPath);
                    if (!result)
                    {
                        await Application.Current.MainPage.DisplayAlert("エラー", "ファイルが取得できませんでした", "閉じる");
                        return;
                    }
                    dlResult = DependencyService.Get<IFileService>().SaveImageiOSLibrary(mediaFolderPath);
                }
                else
                {
                    dlResult = DependencyService.Get<IFileService>().SaveImageiOSLibrary(imageUrl);
                }
            }

            if (dlResult == true)
                await Application.Current.MainPage.DisplayAlert("", "保存しました", "閉じる");
            else if (dlResult == false)
                await Application.Current.MainPage.DisplayAlert("", "保存できませんでした", "閉じる");
        }
    }
}
