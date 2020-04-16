using System;
namespace Entap.Chat
{
    /// <summary>
    /// メッセージタイプ
    /// </summary>
    public enum MessageType
    {
        None,
        Text,
        Image,
        Movie,
        MemberAddRoom=10,
        MemberLeaveRoom=11,
        Custom=100
    }
}
