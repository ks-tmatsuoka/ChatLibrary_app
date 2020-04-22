using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Entap.Chat;
using System.Linq;
using Xamarin.Forms;
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
                    //var msgBase = new MessageBase
                    //{
                    //    MessageId = val.MessageId,
                    //    Text = val.Text,
                    //    SendUserId = val.SendUserId,
                    //    SendDateTime = val.SendDateTime,
                    //    MediaUrl = val.MediaUrl,
                    //    MessageType = val.MessageType,
                    //    AlreadyReadCount = val.AlreadyReadCount,
                    //    UserIcon = members.Where(w => w.UserId == val.SendUserId).LastOrDefault()?.UserIcon
                    //};
                    // もし既読の表示をつけたくない時はここで0を強制的に入れる
                    //msgBase.AlreadyReadCount = 0;

                    val.UserIcon = members.Where(w => w.UserId == val.SendUserId).LastOrDefault()?.UserIcon;
                    messages.Add(val);
                }
            }
            else
            {
                // 通信失敗時はnull返す
                return await Task.FromResult<IEnumerable<MessageBase>>(null);
            }
            
            if (messageDirection == (int)MessageDirection.Old)
                messages.Reverse();
            return await Task.FromResult<IEnumerable<MessageBase>>(messages);
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

        /// <summary>
        /// データの更新
        /// </summary>
        public IDisposable subscription;
        public void UpdateData(ObservableCollection<MessageBase> messageBases, int roomId, List<ChatMemberBase> members)
        {
            DefaultWampChannelFactory channelFactory = new DefaultWampChannelFactory();

            IWampChannel<JToken> channel = channelFactory.CreateChannel("wss://chat.entap.dev:8080");

            channel.Open();

            // PubSub subscription:
            ISubject<JToken> subject = channel.GetSubject<JToken>(roomId.ToString());
            subscription = subject.Subscribe(x =>
            {
                System.Diagnostics.Debug.WriteLine(x);
                var data = JsonConvert.DeserializeObject<WebSocketReceiveDataBase>(x.ToString());
                if (!string.IsNullOrEmpty(data.Message))
                {
                    var msg = JsonConvert.DeserializeObject<MessageBase>(data.Message);
                    if (msg.SendUserId == GetUserId())
                        return;
                    msg.UserIcon = members.Where(w => w.UserId == msg.SendUserId)?.LastOrDefault()?.UserIcon;
                    messageBases.Add(msg);
                }
                if (!string.IsNullOrEmpty(data.AlreadyReadInfomation))
                {
                    var info = JsonConvert.DeserializeObject<WebSocketReciveAlreadyReadInfomation>(data.AlreadyReadInfomation);
                    var msg = messageBases.Where(w => w.MessageId == info.MessageId).LastOrDefault();
                    if (msg != null)
                        msg.AlreadyReadCount = info.AlreadyReadCount;
                }
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
            var delItem = mg.GetItem(id);
            if (delItem != null && !string.IsNullOrEmpty(delItem.MediaUrl))
                FileManager.FileDelete(delItem.MediaUrl);
            mg.DeleteItem(id);
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
        /// 送信する画像を保存するフォルダのパスを取得
        /// </summary>
        /// <returns></returns>
        public string GetSendImageSaveFolderPath()
        {
            var path = FileManager.GetContentsPath(FileManager.AppDataFolders.SendImage) + "/";
            return path;
        }

        /// <summary>
        /// 送信する画像を保存するフォルダのパスを取得
        /// </summary>
        /// <returns></returns>
        public string GetSendVideoSaveFolderPath()
        {
            var path = FileManager.GetContentsPath(FileManager.AppDataFolders.SendVideo) + "/";
            return path;
        }
    }
}
