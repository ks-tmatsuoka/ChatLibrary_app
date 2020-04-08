using System;
namespace ChatSample
{
    public class RespAddDevice : ResponseBase
    {
        public Contents Data { get; set; }
        public class Contents
        {
            public string Udid { get; set; }
        }
    }
}
