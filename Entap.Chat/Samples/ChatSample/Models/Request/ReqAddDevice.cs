using System;
using Xamarin.Forms;

namespace ChatSample
{
    public class ReqAddDevice : ReqUdid
    {
        public string AppVersion
        {
            get
            {
                return "0.0.1";
            }
        }
        public int Platform
        {
            get
            {
                if (Device.RuntimePlatform == Device.iOS)
                {
                    return 1;
                }
                else
                {
                    return 2;
                }
            }
        }
        public string Token { get; set; } // DeviceToken
    }
}
