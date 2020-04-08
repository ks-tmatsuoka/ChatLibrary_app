using System;
namespace ChatSample
{
    public class RespUserId : ResponseBase
    {
        public Contents Data { get; set; }
        public class Contents
        {
            public string UserId { get; set; }
        }
    }
}
