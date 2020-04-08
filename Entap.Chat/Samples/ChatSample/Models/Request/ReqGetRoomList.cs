using System;
namespace ChatSample
{
    public class ReqGetRoomList
    {
        public string UserId
        {
            get
            {
                return UserDataManager.Instance.UserId;
            }
        }
    }
}
