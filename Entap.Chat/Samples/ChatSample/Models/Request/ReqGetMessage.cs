using System;
namespace ChatSample
{
    public class ReqGetMessage
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
        public int MessageDirection { get; set; }
        public int Conut { get; set; }
    }
}
