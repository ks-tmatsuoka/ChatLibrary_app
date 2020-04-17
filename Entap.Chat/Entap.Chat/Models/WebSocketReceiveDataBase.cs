using System;
namespace Entap.Chat
{
    public class WebSocketReceiveDataBase
    {
        public int RoomId { get; set; }
        public string Message { get; set; }
        public string AlreadyReadInfomation { get; set; }
    }
}
