using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Entap.Chat;
using System.Linq;
using Xamarin.Forms;

namespace ChatSample
{
    public class ChatService : IChatService
    {
        const int LoadCount = 20;
        public Task<IEnumerable<MessageBase>> GetMessagesAsync(int id)
        {
            if (id == 0) return null;

            var messages = new List<MessageBase>();
            for (int i = 0; i < LoadCount; i++)
            {
                if (id - i < 0) break;

                var mod = (id - i) % 3;
                if (mod == 0)
                    messages.Add(new MessageBase { MessageId = id - i, ImageUrl = "http://placehold.jp/50x50.png?text=" + (id - i), MessageType = 2 });
                else
                    messages.Add(new MessageBase { MessageId = id - i, Text = (id - i).ToString(), MessageType = 1 });
            }
            messages.Reverse();
            return Task.FromResult<IEnumerable<MessageBase>>(messages);
        }

        public Task<IEnumerable<MessageBase>> GetNewMessagesAsync(int id)
        {
            var messages = new List<MessageBase>();
            if (id >= 120 || id <= 0)
                return Task.FromResult<IEnumerable<MessageBase>>(messages);

            for (int i = 0; i < LoadCount; i++)
            {
                if (id - i < 0) break;

                var mod = (id - i) % 3;
                if (mod == 0)
                    messages.Add(new MessageBase { MessageId = id + i, ImageUrl = "http://placehold.jp/50x50.png?text=" + (id + i), MessageType = 2 });
                else
                    messages.Add(new MessageBase { MessageId = id + i, Text = (id + i).ToString(), MessageType = 1 });

            }
            return Task.FromResult<IEnumerable<MessageBase>>(messages);
        }

        public Task<int> SendMessage(MessageBase msg)
        {
            var time = DateTime.Now;
            int i;
            if (time.Second % 2 == 0)
                i = 0;
            else
                i = -1;
            return Task.FromResult<int>(i);
        }

        public Task<int> SendAlreadyRead(int msgId)
        {
            int i;
            i = 0;
            return Task.FromResult<int>(i);
        }

        public async Task<string> TakePicture()
        {
            var mg = new MediaPluginManager();
            var path = await mg.TakePhotoAsync();
            if (path is null)
                return await Task.FromResult<string>("");
            return await Task.FromResult<string>(path);
        }

        public async Task<string> SelectImage()
        {
            var mg = new MediaPluginManager();
            var paths = await mg.PickPhotoAsyncGetPathAndAlbumPath();
            if (paths is null)
                return await Task.FromResult<string>("");
            byte[] bytes = null;
            string extension = "";
            string sendImgUrl = "";
            if (!string.IsNullOrEmpty(paths[0]))
            {
                sendImgUrl = paths[0];
                bytes = FileManager.ReadBytes(sendImgUrl);
                if ((bytes == null || bytes.Length < 1) && paths.Count > 1)
                {
                    sendImgUrl = paths[1];
                    bytes = FileManager.ReadBytes(sendImgUrl);
                    extension = System.IO.Path.GetExtension(sendImgUrl);
                }
            }

            if (bytes == null || bytes.Length < 1)
            {
                if (bytes != null && bytes.Length < 1)
                    await App.Current.MainPage.DisplayAlert(null, "こちらの画像は送信できません", "閉じる");
                return await Task.FromResult<string>("");
            }

            return await Task.FromResult<string>(sendImgUrl);
        }

        public void UpdateData(ObservableCollection<MessageBase> messageBases)
        {
            Task.Run(async () =>
            {
                // 既読つける処理
                while (true)
                {
                    await Task.Delay(2000);
                    var msgs = messageBases.Where(w => w.MessageId >= 120);
                    foreach (var msg in msgs)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            msg.IsAlreadyRead = true;
                        });
                    }
                }
            });

            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(10000);

                    Device.BeginInvokeOnMainThread(() =>
                    {
                        var id = messageBases.Max(w => w.MessageId) + 1;
                        //messageBases.Add(new MessageBase { MessageId = id, Text = "other", IsAlreadyRead = false, MessageType = 1 });
                    });
                }

            });
        }

        /// <summary>
        /// チャットのリストの末尾に送信できていないメッセージを追加
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="messageBases"></param>
        public void AddNotSendMessages(int roomId, ObservableCollection<MessageBase> messageBases)
        {
            var items = new NotSendMessageManager().GetItems(roomId);
            foreach (var item in items)
            {
                var messageBase = new MessageBase(item);
                messageBase.NotSendId = item.Id;
                messageBases.Add(messageBase);
            }
        }

        /// <summary>
        /// 送れていないメッセージをStorageに保存
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="messageBase"></param>
        /// <returns></returns>
        public void SaveNotSendMessageData(int roomId, MessageBase messageBase)
        {
            var mg = new NotSendMessageManager();
            var notSendMessage = new NotSendMessage(roomId, messageBase);
            var id = mg.SaveItem(notSendMessage);
            // sqliteのデータととチャットのメッセージデータの紐付け
            messageBase.NotSendId = id;
        }

        /// <summary>
        /// ストレージに保存されてる未送信データの削除
        /// </summary>
        /// <param name="id">プライマリーキー</param>
        public void DeleteNotSendMessageData(int id)
        {
            var mg = new NotSendMessageManager();
            mg.DeleteItem(id);
        }


        public async Task ImageShare(string imagePath)
        {
            FileManager.CreateFolders();
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

        public async Task ImageDownload(string imageUrl)
        {
            FileManager.CreateFolders();
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
                dlResult = await ImageManager.DownloadWebImageFile(imageUrl, filePath);
            else
                dlResult = DependencyService.Get<IFileService>().SaveImageiOSLibrary(imageUrl);
            if (dlResult == true)
                await Application.Current.MainPage.DisplayAlert("", "保存しました", "閉じる");
            else if (dlResult == false)
                await Application.Current.MainPage.DisplayAlert("", "保存できませんでした", "閉じる");
        }

        public string GetSendImageSaveFolderPath()
        {
            var path = FileManager.GetContentsPath(FileManager.AppDataFolders.SendImage) + "/";
            return path;
        }

        public string GetNotSendImageSaveFolderPath()
        {
            var path = FileManager.GetContentsPath(FileManager.AppDataFolders.NotSendImage) + "/";
            return path;
        }

        public string GetUserId()
        {
            return UserDataManager.Instance.UserId;
        }
    }
}
