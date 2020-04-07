using System;
using SQLite;

namespace Entap.Chat
{
    /// <summary>
    /// 未送信メッセージ
    /// </summary>
    public class NotSendMessage : MessageBase
    {
        public NotSendMessage()
        {
        }
        public NotSendMessage(int roomId,MessageBase messageBase)
        {
            MessageId = messageBase.MessageId;
            DateTime = messageBase.DateTime;
            Text = messageBase.Text;
            ImageUrl = messageBase.ImageUrl;
            UserIcon = messageBase.UserIcon;
            MessageType = messageBase.MessageType;
            SendUserId = messageBase.SendUserId;
            IsAlreadyRead = false;
            ResendVisible = true;
            RoomId = roomId;
        }
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int RoomId { get; set; }
    }
}
