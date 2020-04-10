using System;
using System.Collections.Generic;

namespace ChatSample
{
    public class ResqGetContactAddresses : ResponseBase
    {
        public Contents Data { get; set; }
        public class Contents
        {
            public List<ContactAddress> ContactAddresses { get; set; }
        }
        public class ContactAddress
        {
            public string UserId { get; set; }
            public string UserIcon { get; set; }
            public string UserName { get; set; }
        }
    }
}
