using System;
namespace ChatSample
{
    public class ReqGetUserId : ReqUdid
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
