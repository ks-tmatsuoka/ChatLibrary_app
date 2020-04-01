using System;
using System.Net;
using System.Threading.Tasks;

namespace Entap.Chat
{
    public class ImageManager
    {
        public ImageManager()
        {
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
