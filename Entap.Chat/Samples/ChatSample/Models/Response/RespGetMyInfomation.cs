using System;
namespace ChatSample
{
    public class RespGetMyInfomation : ResponseBase
    {
        public Contents Data { get; set; }
        public class Contents
        {
            public string ContactId { get; set; }
            public string UserName { get; set; }
            public string UserIcon { get; set; }
        }
    }
}
