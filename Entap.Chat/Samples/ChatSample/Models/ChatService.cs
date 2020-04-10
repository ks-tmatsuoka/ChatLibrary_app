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
                        ImageUrl = val.MediaUrl,
                        MessageType = val.MessageType,
                        UserIcon = members.Where(w => w.UserId == val.SendUserId).LastOrDefault()?.UserIcon
                    };
                    if (val.AlreadyReadCount > 0 && members.Count - 1 >= val.AlreadyReadCount)
                    {
                        msgBase.IsAlreadyRead = true;
                    }
                    messages.Add(msgBase);
                }
            }
            if (messageDirection == (int)MessageDirection.Old)
                messages.Reverse();
            return await Task.FromResult<IEnumerable<MessageBase>>(messages);
        }

        /// <summary>
        /// メッセージ送信
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
            if (msg.MessageType == (int)MessageType.Image && !string.IsNullOrEmpty(msg.ImageUrl))
            {
                // 画像
                bytes = FileManager.ReadBytes(msg.ImageUrl);
                var extension = System.IO.Path.GetExtension(msg.ImageUrl);
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

            /*
            Task.Run(async () =>
            {
                //クライアント側のWebSocketを定義
                ClientWebSocket ws = new ClientWebSocket();

                //接続先エンドポイントを指定
                var uri = new Uri("wss://chat.entap.dev:8080");

                //サーバに対し、接続を開始
                await ws.ConnectAsync(uri, CancellationToken.None);
                var buffer = new byte[1024];

                while (true)
                {
                    //await Task.Delay(10000);

                    //Device.BeginInvokeOnMainThread(() =>
                    //{
                    //    var id = messageBases.Max(w => w.MessageId) + 1;
                    //    //messageBases.Add(new MessageBase { MessageId = id, Text = "other", IsAlreadyRead = false, MessageType = 1 });
                    //});


                    //所得情報確保用の配列を準備
                    var segment = new ArraySegment<byte>(buffer);

                    //サーバからのレスポンス情報を取得
                    var result = await ws.ReceiveAsync(segment, CancellationToken.None);

                    //エンドポイントCloseの場合、処理を中断
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "OK",
                          CancellationToken.None);
                        return;
                    }

                    //バイナリの場合は、当処理では扱えないため、処理を中断
                    if (result.MessageType == WebSocketMessageType.Binary)
                    {
                        await ws.CloseAsync(WebSocketCloseStatus.InvalidMessageType,
                          "I don't do binary", CancellationToken.None);
                        return;
                    }

                    //メッセージの最後まで取得
                    int count = result.Count;
                    while (!result.EndOfMessage)
                    {
                        if (count >= buffer.Length)
                        {
                            await ws.CloseAsync(WebSocketCloseStatus.InvalidPayloadData,
                              "That's too long", CancellationToken.None);
                            return;
                        }
                        segment = new ArraySegment<byte>(buffer, count, buffer.Length - count);
                        result = await ws.ReceiveAsync(segment, CancellationToken.None);

                        count += result.Count;
                    }

                    //メッセージを取得
                    var message = System.Text.Encoding.UTF8.GetString(buffer, 0, count);
                    Console.WriteLine("> " + message);
                }

            });
            */
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
        /// ファイルの共有
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        public async Task ImageShare(string imagePath)
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

        /// <summary>
        /// 画像を端末にダウンロード(Androidはダウンロードフォルダ、iOSはカメラロール)
        /// </summary>
        /// <param name="imageUrl"></param>
        /// <returns></returns>
        public async Task ImageDownload(string imageUrl)
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
