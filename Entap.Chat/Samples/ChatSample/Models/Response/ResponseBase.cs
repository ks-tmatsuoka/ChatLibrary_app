using System;
using System.Collections.Generic;

namespace ChatSample
{
    public class ResponseBase
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
    }
}
