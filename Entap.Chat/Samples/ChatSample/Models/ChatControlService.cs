using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entap.Chat;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ChatSample
{
    public class ChatControlService : IChatControlService
    {
        /// <summary>
        /// メッセージ送信
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task<SendMessageResponseBase> SendMessage(int roomId, MessageBase msg, int notSendMessageId)
        {
            var dic = new Dictionary<string, string>();
            dic["RoomId"] = roomId.ToString();
            dic["UserId"] = UserDataManager.Instance.UserId;
            dic["MessageType"] = msg.MessageType.ToString();
            if (!string.IsNullOrEmpty(msg.Text))
                dic["Text"] = msg.Text;

            byte[] bytes = null;
            string name = "";
            string fileType = "";
            if (msg.MessageType == (int)MessageType.Image && !string.IsNullOrEmpty(msg.MediaUrl))
            {
                // 画像
                bytes = FileManager.ReadBytes(msg.MediaUrl);
                var extension = System.IO.Path.GetExtension(msg.MediaUrl);
                name = Guid.NewGuid().ToString() + extension;
                fileType = "Image";
                if (bytes == null || bytes.Length < 1)
                {
                    await App.Current.MainPage.DisplayAlert("この写真は送れません", "", "閉じる");
                    return await Task.FromResult<SendMessageResponseBase>(new SendMessageResponseBase { MessageId = notSendMessageId });
                }
            }
            else if (msg.MessageType == (int)MessageType.Video)
            {
                bytes = FileManager.ReadBytes(msg.MediaUrl);
                var extension = System.IO.Path.GetExtension(msg.MediaUrl);
                name = Guid.NewGuid().ToString() + extension;
                fileType = "Movie";
                if (bytes == null || bytes.Length < 1)
                {
                    await App.Current.MainPage.DisplayAlert("この動画は送れません", "", "閉じる");
                    return await Task.FromResult<SendMessageResponseBase>(new SendMessageResponseBase { MessageId = notSendMessageId });
                }
            }

            var json = await APIManager.PostFile(APIManager.GetEntapAPI(APIManager.EntapAPIName.SendMessage), bytes, name, dic, fileType);
            var resp = JsonConvert.DeserializeObject<RespSendMessage>(json);
            if (resp.Status == APIManager.APIStatus.Succeeded)
                return await Task.FromResult<SendMessageResponseBase>(resp.Data);

            return await Task.FromResult<SendMessageResponseBase>(new SendMessageResponseBase { MessageId = notSendMessageId });
        }

        /// <summary>
        /// 写真撮影
        /// </summary>
        /// <returns></returns>
        public async Task<string> TakePicture()
        {
            var mg = new MediaPluginManager();
            var path = await mg.TakePhotoAsync();
            if (path is null)
                return await Task.FromResult<string>("");
            return await Task.FromResult<string>(path);
        }

        /// <summary>
        /// 動画撮影
        /// </summary>
        /// <returns></returns>
        public async Task<string> TakeVideo()
        {
            var mg = new MediaPluginManager();
            var path = await mg.TakeVideoAsync();
            if (path is null)
                return await Task.FromResult<string>("");
            return await Task.FromResult<string>(path);
        }

        /// <summary>
        /// 動画撮影
        /// </summary>
        /// <returns></returns>
        public async Task<string> SelectVideo()
        {
            var mg = new MediaPluginManager();
            var path = await mg.PickVideoAsync();
            if (path is null)
                return await Task.FromResult<string>("");
            return await Task.FromResult<string>(path);
        }

        /// <summary>
        /// ライブラリからの画像選択
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 画像ファイルの共有
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public async Task ImageShare(string imagePath)
        {
            await MediaManager.ImageShare(imagePath);
        }

        /// <summary>
        /// 動画ファイルの共有
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public async Task VideoShare(string videoPath)
        {
            await MediaManager.VideoShare(videoPath);
        }

        /// <summary>
        /// 送信できなかった画像を保存するフォルダのパスを取得
        /// </summary>
        /// <returns></returns>
        public string GetNotSendImageSaveFolderPath()
        {
            var path = FileManager.GetContentsPath(FileManager.AppDataFolders.NotSendImage) + "/";
            return path;
        }

        /// <summary>
        /// 画像のプレビューページへ遷移
        /// </summary>
        /// <param name="imageUrl"></param>
        public void MoveImagePreviewPage(string imageUrl)
        {
            App.Current.MainPage.Navigation.PushModalAsync(new ImagePreviewPage(imageUrl));
        }

        /// <summary>
        /// 動画のプレビューページへ遷移
        /// </summary>
        /// <param name="imageUrl"></param>
        public void MoveVideoPreviewPage(string imageUrl)
        {
            App.Current.MainPage.Navigation.PushModalAsync(new VideoPreviewPage(imageUrl));
        }

        /// <summary>
        /// BottomControllerの各メニュー押した際の動作指定
        /// </summary>
        /// <param name="notSendMessageId"></param>
        /// <param name="type"></param>
        /// <param name="roomId"></param>
        /// <param name="chatListView"></param>
        /// <returns></returns>
        public async Task<IEnumerable<MessageBase>> BottomControllerMenuExecute(int notSendMessageId, int type, int roomId, ChatListView chatListView)
        {
            string photoStr = "写真";
            string VideoStr = "動画";
            if (type == (int)BottomControllerMenuType.Camera)
            {
                var selected = await App.Current.MainPage.DisplayActionSheet("選択してください", null, null, new string[2] { photoStr, VideoStr });
                if (selected == photoStr)
                {
                    var imgPath = await TakePicture();
                    if (string.IsNullOrEmpty(imgPath))
                        return await Task.FromResult<IEnumerable<MessageBase>>(null);
                    byte[] bytes = FileManager.ReadBytes(imgPath);
                    string extension = System.IO.Path.GetExtension(imgPath);
                    string name = Guid.NewGuid().ToString() + extension;
                    if (bytes == null || bytes.Length < 1)
                    {
                        return await Task.FromResult<IEnumerable<MessageBase>>(null);
                    }
                    var copyImgPath = Settings.Current.ChatService.GetSendImageSaveFolderPath() + Guid.NewGuid() + extension;
                    if (!FileManager.FileCopy(imgPath, copyImgPath))
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Application.Current.MainPage.DisplayAlert("", "画像の取得に失敗しました", "閉じる");
                        });
                        return await Task.FromResult<IEnumerable<MessageBase>>(null);
                    }
                    var msg = new MessageBase { MessageId = notSendMessageId, MediaUrl = copyImgPath, MessageType = (int)MessageType.Image, SendUserId = Settings.Current.ChatService.GetUserId() };
                    return await Task.FromResult<IEnumerable<MessageBase>>(new List<MessageBase> { msg });
                }
                else if (selected == VideoStr)
                {
                    var videoPath = await TakeVideo();
                    if (string.IsNullOrEmpty(videoPath))
                        return await Task.FromResult<IEnumerable<MessageBase>>(null);
                    byte[] bytes = FileManager.ReadBytes(videoPath);
                    string extension = ".mp4";
                    string name = Guid.NewGuid().ToString() + extension;
                    if (bytes == null || bytes.Length < 1)
                    {
                        return await Task.FromResult<IEnumerable<MessageBase>>(null);
                    }
                    var copyPath = Settings.Current.ChatService.GetSendVideoSaveFolderPath() + Guid.NewGuid() + extension;
                    bool result;
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        result = DependencyService.Get<IVideoService>().ConvertMp4(videoPath, copyPath);
                    }
                    else
                    {
                        result = FileManager.FileCopy(videoPath, copyPath);
                    }
                    if (result)
                    {
                        var msg = new MessageBase { MessageId = notSendMessageId, MediaUrl = copyPath, MessageType = (int)MessageType.Video, SendUserId = Settings.Current.ChatService.GetUserId() };
                        return await Task.FromResult<IEnumerable<MessageBase>>(new List<MessageBase> { msg });
                    }
                    return await Task.FromResult<IEnumerable<MessageBase>>(null);
                    
                }
            }
            else if (type == (int)BottomControllerMenuType.Library)
            {
                var selected = await App.Current.MainPage.DisplayActionSheet("選択してください", null, null, new string[2] { photoStr, VideoStr });
                if (selected == photoStr)
                {
                    var imgPath = await SelectImage();
                    if (string.IsNullOrEmpty(imgPath))
                        return await Task.FromResult<IEnumerable<MessageBase>>(null);
                    byte[] bytes = FileManager.ReadBytes(imgPath);
                    string extension = System.IO.Path.GetExtension(imgPath);
                    string name = Guid.NewGuid().ToString() + extension;
                    if (bytes == null || bytes.Length < 1)
                    {
                        return await Task.FromResult<IEnumerable<MessageBase>>(null);
                    }
                    var copyImgPath = Settings.Current.ChatService.GetSendImageSaveFolderPath() + Guid.NewGuid() + extension;
                    if (!FileManager.FileCopy(imgPath, copyImgPath))
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            Application.Current.MainPage.DisplayAlert("", "画像の取得に失敗しました", "閉じる");
                        });
                        return await Task.FromResult<IEnumerable<MessageBase>>(null);
                    }
                    var msg = new MessageBase { MessageId = notSendMessageId, MediaUrl = copyImgPath, MessageType = (int)MessageType.Image, SendUserId = Settings.Current.ChatService.GetUserId() };
                    return await Task.FromResult<IEnumerable<MessageBase>>(new List<MessageBase> { msg });
                }
                else if (selected == VideoStr)
                {
                    var videoPath = await SelectVideo();
                    if (string.IsNullOrEmpty(videoPath))
                        return await Task.FromResult<IEnumerable<MessageBase>>(null);
                    byte[] bytes = FileManager.ReadBytes(videoPath);
                    string extension = ".mp4";
                    string name = Guid.NewGuid().ToString() + extension;
                    if (bytes == null || bytes.Length < 1)
                    {
                        return await Task.FromResult<IEnumerable<MessageBase>>(null);
                    }
                    var copyPath = Settings.Current.ChatService.GetSendVideoSaveFolderPath() + Guid.NewGuid() + extension;
                    bool result;
                    if (Device.RuntimePlatform == Device.iOS)
                    {
                        result = DependencyService.Get<IVideoService>().ConvertMp4(videoPath, copyPath);
                    }
                    else
                    {
                        result = FileManager.FileCopy(videoPath, copyPath);
                    }
                    if (result)
                    {
                        var msg = new MessageBase { MessageId = notSendMessageId, MediaUrl = copyPath, MessageType = (int)MessageType.Video, SendUserId = Settings.Current.ChatService.GetUserId() };
                        return await Task.FromResult<IEnumerable<MessageBase>>(new List<MessageBase> { msg });
                    }
                    return await Task.FromResult<IEnumerable<MessageBase>>(null);
                }
            }
            return await Task.FromResult<IEnumerable<MessageBase>>(null);
        }
    }
}
