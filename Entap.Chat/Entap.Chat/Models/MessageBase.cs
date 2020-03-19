using System;
namespace Entap.Chat
{
    /// <summary>
    /// メッセージ
    /// </summary>
    public class MessageBase
    {
        public MessageBase(MessageType messageType = MessageType.None)
        {
            MessageType = messageType;
        }
        public MessageType MessageType { get; private set; }

        public int Id { get; set; }
    }
}
