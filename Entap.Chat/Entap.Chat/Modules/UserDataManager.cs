using System;
namespace Entap.Chat
{
    public class UserDataManager
    {
        public static UserDataManager Instance => _instance;
        private static readonly UserDataManager _instance = new UserDataManager();
        public UserDataManager()
        {
        }

        //public string UserId { get; set; }
        public string UserId = "abc";
    }
}
