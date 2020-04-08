using System;
using SQLite;

namespace ChatSample
{
    public class UserData
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Udid { get; set; }
        public string UserId { get; set; }
    }
}
