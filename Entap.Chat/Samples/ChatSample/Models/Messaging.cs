using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Entap.Chat;
using System.Linq;
using Xamarin.Forms;
using System.Text.RegularExpressions;

namespace ChatSample
{
    public class Messaging : IMessaging
    {
        public Task<IEnumerable<MessageBase>> GetMessagesAsync(int id, int count)
        {
            if (id == 0) return null;

            var messages = new List<MessageBase>();
            for (int i = 0; i < count; i++)
            {
                if (id - i < 0) break;

                var mod = (id - i) % 3;
                if (mod == 0)
                    messages.Add(new MessageBase { MessageId = id - i, ImageUrl = "http://placehold.jp/50x50.png?text=" + (id - i), MessageType=2 });
                else
                    messages.Add(new MessageBase { MessageId = id - i, Text= (id - i).ToString(), MessageType=1});
            }
            messages.Reverse();
            return Task.FromResult<IEnumerable<MessageBase>>(messages);
        }

        public Task<IEnumerable<MessageBase>> GetNewMessagesAsync(int id, int count)
        {
            var messages = new List<MessageBase>();
            if (id >= 120 || id <= 0)
                return Task.FromResult<IEnumerable<MessageBase>>(messages);

            for (int i = 0; i < count; i++)
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
                while(true)
                {
                    await Task.Delay(2000);
                    var msgs = messageBases.Where(w => w.MessageId >= 120);
                    foreach(var msg in msgs)
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
                        var id = messageBases.Max(w=>w.MessageId) + 1;
                        messageBases.Add(new MessageBase { MessageId = id, Text = "other", IsAlreadyRead = false, MessageType = 1 });
                    });
                }
                
            });
        }
    }
}
