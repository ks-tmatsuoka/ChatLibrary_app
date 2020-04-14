using System;
namespace ChatSample
{
    public class ReqLeaveRoom
    {
        public string UserId
        {
            get
            {
                return UserDataManager.Instance.UserId;
            }
        }
        public int RoomId { get; set; }
    }
}
