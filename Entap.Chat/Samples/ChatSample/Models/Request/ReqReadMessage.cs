using System;
namespace ChatSample
{
    public class ReqReadMessage
    {
        public string UserId
        {
            get
            {
                return UserDataManager.Instance.UserId;
            }
        }
        public int RoomId { get; set; }
        public int MessageId { get; set; }
    }
}
