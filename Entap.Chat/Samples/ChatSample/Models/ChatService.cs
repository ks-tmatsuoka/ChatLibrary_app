using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Entap.Chat;
using System.Linq;
using Xamarin.Forms;
using System.Net.WebSockets;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reactive.Subjects;
using WampSharp.V1;

namespace ChatSample
{
    public class ChatService : IChatService
    {
        const int LoadCount = 20;

        /// <summary>
        /// メッセージ取得
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="messageId"></param>
        /// <param name="messageDirection"></param>
        /// <param name="members"></param>
        /// <returns></returns>
        public async Task<IEnumerable<MessageBase>> GetMessagesAsync(int roomId, int messageId, int messageDirection, List<ChatMemberBase> members)
        {
            var req = new ReqGetMessage
            {
                RoomId = roomId,
                MessageId = messageId,
                MessageDirection = messageDirection,
                Conut = LoadCount
            };
            var json = await APIManager.PostAsync(APIManager.GetEntapAPI(APIManager.EntapAPIName.GetMessages), req);
            var resp = JsonConvert.DeserializeObject<RespGetMessage>(json);
            var messages = new List<MessageBase>();
            if (resp.Status == APIManager.APIStatus.Succeeded)
            {
                foreach (var val in resp.Data.MessageList)
                {
                    var msgBase = new MessageBase
                    {
                        MessageId = val.MessageId,
                        Text = val.Text,
                        SendUserId = val.SendUserId,
                        DateTime = val.SendDateTime,
                        MediaUrl = val.MediaUrl,
                        MessageType = val.MessageType,
                        AlreadyReadCount = val.AlreadyReadCount,
                        UserIcon = members.Where(w => w.UserId == val.SendUserId).LastOrDefault()?.UserIcon
                    };
                    // もし既読の表示をつけたくない時はここで0を強制的に入れる
                    //msgBase.AlreadyReadCount = 0;
                    messages.Add(msgBase);
                }
            }
            
            if (messageDirection == (int)MessageDirection.Old)
                messages.Reverse();
            return await Task.FromResult<IEnumerable<MessageBase>>(messages);
        }

        /// <summary>
        /// メッセージ送信/ChatControlのみで使用
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task<SendMessageResponseBase> SendMessage(int roomId, MessageBase msg)
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
                    return await Task.FromResult<SendMessageResponseBase>(new SendMessageResponseBase { MessageId=-1 });
                }
            }
            else if (msg.MessageType == (int)MessageType.Movie)
            {
                //TODO 動画
                fileType = "Movie";
                return await Task.FromResult<SendMessageResponseBase>(new SendMessageResponseBase { MessageId = -1 });
            }

            var json = await APIManager.PostFile(APIManager.GetEntapAPI(APIManager.EntapAPIName.SendMessage), bytes, name, dic, fileType);
            var resp = JsonConvert.DeserializeObject<RespSendMessage>(json);
            if (resp.Status == APIManager.APIStatus.Succeeded)
                return await Task.FromResult<SendMessageResponseBase>(resp.Data);

            return await Task.FromResult<SendMessageResponseBase>(new SendMessageResponseBase { MessageId = -1 });
        }

        /// <summary>
        /// 既読をサーバ側に知らせる
        /// </summary>
        /// <param name="roomId"></param>
        /// <param name="messageId"></param>
        /// <returns></returns>
        public async Task<bool> SendAlreadyRead(int roomId, int messageId)
        {
            bool result = false;
            var req = new ReqReadMessage
            {
                RoomId = roomId,
                MessageId = messageId
            };
            var json = await APIManager.PostAsync(APIManager.GetEntapAPI(APIManager.EntapAPIName.ReadMessage), req);
            var resp = JsonConvert.DeserializeObject<ResponseBase>(json);
            if (resp.Status == APIManager.APIStatus.Succeeded)
            {
                result = true;
            }
            return await Task.FromResult<bool>(result);
        }

        /// <summary>
        /// 写真撮影 / ChatControlのみで使用
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
        /// ライブラリからの画像選択 / ChatControlのみで使用
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
        /// ルームのメンバー取得
        /// </summary>
        /// <param name="roomId"></param>
        public async Task<List<ChatMemberBase>> GetRoomMembers(int roomId)
        {
            var req = new ReqGetRoomMembers
            {
                RoomId = roomId
            };
            var list = new List<ChatMemberBase>();
            var json = await APIManager.PostAsync(APIManager.GetEntapAPI(APIManager.EntapAPIName.GetRoomMembers), req);
            var resp = JsonConvert.DeserializeObject<RespGetRoomMembers>(json);
            if (resp.Status == APIManager.APIStatus.Succeeded)
            {
                foreach (var member in resp.Data.Members)
                {
                    list.Add(member);
                }
            }
            return list;
        }

        public IDisposable subscription;
        public void UpdateData(ObservableCollection<MessageBase> messageBases, int roomId)
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
                            msg.AlreadyReadCount = 1;
                        });
                    }
                }
            });


            DefaultWampChannelFactory channelFactory = new DefaultWampChannelFactory();

            IWampChannel<JToken> channel = channelFactory.CreateChannel("wss://chat.entap.dev:8080");

            channel.Open();

            // PubSub subscription:
            ISubject<JToken> subject = channel.GetSubject<JToken>(roomId.ToString());
            //var s = subscription;
            subscription = subject.Subscribe(x =>
            {
                System.Diagnostics.Debug.WriteLine(x);
                //var data = JsonConvert.DeserializeObject<WebSocketReceiveDataBase>(x.ToString());
                //if (!string.IsNullOrEmpty(data.Message))
                //{

                //}
                //if (!string.IsNullOrEmpty(data.AlreadyReadInfomation))
                //{

                //}
            });
        }

        public void Dispose()
        {
            if (subscription is null)
                return;
            subscription.Dispose();
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

        /// <summary>
        /// ファイルの共有 / ChatControlで使用
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public async Task ImageShare(string imagePath)
        {
            await ImageManager.ImageShare(imagePath);
        }

        /// <summary>
        /// ChatControlで使用
        /// </summary>
        /// <returns></returns>
        public string GetSendImageSaveFolderPath()
        {
            var path = FileManager.GetContentsPath(FileManager.AppDataFolders.SendImage) + "/";
            return path;
        }

        /// <summary>
        /// ChatControlで使用
        /// </summary>
        /// <returns></returns>
        public string GetNotSendImageSaveFolderPath()
        {
            var path = FileManager.GetContentsPath(FileManager.AppDataFolders.NotSendImage) + "/";
            return path;
        }

        /// <summary>
        /// UserId取得
        /// </summary>
        /// <returns></returns>
        public string GetUserId()
        {
            return UserDataManager.Instance.UserId;
        }

        /// <summary>
        /// 画像のプレビューページへ遷移 / ChatControlで使用
        /// </summary>
        /// <param name="imageUrl"></param>
        public void MoveImagePreviewPage(string imageUrl)
        {
            App.Current.MainPage.Navigation.PushModalAsync(new ImagePreviewPage(imageUrl));
        }


        /// <summary>
        /// BottomControllerの各メニュー押した際の動作指定 / ChatControlで使用
        /// </summary>
        /// <param name="notSendMessageId"></param>
        /// <param name="type"></param>
        /// <param name="roomId"></param>
        /// <param name="chatListView"></param>
        /// <returns></returns>
        public async Task<IEnumerable<MessageBase>> BottomControllerMenuExecute(int notSendMessageId, int type, int roomId, ChatListView chatListView)
        {
            if (type == (int)BottomControllerMenuType.Camera)
            {
                var imgPath = await Settings.Current.ChatService.TakePicture();
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
            else if (type == (int)BottomControllerMenuType.Library)
            {
                var imgPath = await Settings.Current.ChatService.SelectImage();
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
            else
            {
                /*
                var msg = new MessageBase();
                chatListView.AddMessage(msg);
                var sendMessageResponseBase = await Settings.Current.ChatService.SendMessage(roomId, msg);
                var index = chatListView.Messages.IndexOf(msg);
                if (sendMessageResponseBase.MessageId < 0)
                {
                    // 通信エラー
                    var delImgPath = msg.ImageUrl;
                    string extension = System.IO.Path.GetExtension(delImgPath);
                    chatListView.Messages[index].ResendVisible = true;
                    var sendErrorImgPath = Settings.Current.ChatService.GetNotSendImageSaveFolderPath() + Guid.NewGuid() + extension;
                    FileManager.FileCopy(delImgPath, sendErrorImgPath);
                    chatListView.Messages[index].ImageUrl = sendErrorImgPath;
                    FileManager.FileDelete(delImgPath);
                    chatListView.NotSendMessageSaveInStorage(chatListView.Messages[index]);
                }
                else
                {
                    msg.ResendVisible = false;
                    // サーバへ送信できた段階でメッセージの表示位置を再確認
                    if (index == chatListView.Messages.Count - 1)
                    {
                        chatListView.Messages[index].MessageId = sendMessageResponseBase.MessageId;
                        chatListView.Messages[index].DateTime = sendMessageResponseBase.SendDateTime;
                    }
                    else
                    {
                        chatListView.Messages.RemoveAt(index);
                        msg.MessageId = sendMessageResponseBase.MessageId;
                        msg.DateTime = sendMessageResponseBase.SendDateTime;
                        chatListView.Messages.Add(msg);
                    }
                    chatListView.NotSendMessageDeleteFromStorage(msg.NotSendId);
                }
                */
            }
            return await Task.FromResult<IEnumerable<MessageBase>>(null);
        }
    }
}
