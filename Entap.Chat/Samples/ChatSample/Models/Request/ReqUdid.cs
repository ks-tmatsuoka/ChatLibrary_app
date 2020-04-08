using System;
using ChatSample;

namespace ChatSample
{
    public class ReqUdid
    {
        public string Udid
        {
            get
            {
                return UserDataManager.Instance.Udid;
            }
        }
    }
}
