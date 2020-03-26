using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entap.Chat;
namespace ChatSample
{
    public class Messaging : IMessaging
    {
        public Task<IEnumerable<MessageBase>> GetMessagesAsync(int id, int count)
        {
            if (id == 0) return null;
            if (id < 0)
                id = 100;

            var messages = new List<MessageBase>();
            for (int i = 0; i < count; i++)
            {
                if (id - i < 0) break;

                var mod = (id - i) % 2;
                if (mod == 0)
                    messages.Add(new TextMessage { Id = id - i, Text= (id - i).ToString()});
                else if (mod == 1)
                    messages.Add(new ImageMessage { Id = id - i, ImageUrl= "http://placehold.jp/50x50.png?text=" + (id - i) });
            }
            messages.Reverse();
            return Task.FromResult<IEnumerable<MessageBase>>(messages);
        }

        public Task<IEnumerable<MessageBase>> GetNewMessagesAsync(int id, int count)
        {
            var messages = new List<MessageBase>();
            if (id >= 150)
                return Task.FromResult<IEnumerable<MessageBase>>(messages);

            for (int i = 0; i < count; i++)
            {
                if (id - i < 0) break;

                var mod = (id - i) % 2;
                if (mod == 0)
                    messages.Add(new TextMessage { Id = id + i, Text = (id + i).ToString() });
                else if (mod == 1)
                    messages.Add(new ImageMessage { Id = id + i, ImageUrl = "http://placehold.jp/50x50.png?text=" + (id + i) });
            }
            return Task.FromResult<IEnumerable<MessageBase>>(messages);
        }

        public Task<bool> SendTextMessage(string text)
        {
            return Task.FromResult<bool>(true); ;
        }

        public async Task<string> TakePicture()
        {
            var mg = new MediaPluginManager();
            var path = await mg.TakePhotoAsync();
            return await Task.FromResult<string>(path);
        }

        public async Task<string> SelectImage()
        {
            var mg = new MediaPluginManager();
            var paths = await mg.PickPhotoAsyncGetPathAndAlbumPath();

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
                return await Task.FromResult<string>(""); ;
            }

            return await Task.FromResult<string>(sendImgUrl);
        }

        public Task<bool> SendImage(byte[] imageData)
        {
            return Task.FromResult<bool>(true); ;
        }
    }
}
