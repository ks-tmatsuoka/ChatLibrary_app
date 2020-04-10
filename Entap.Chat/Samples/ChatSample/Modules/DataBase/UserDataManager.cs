using System;
using SQLite;

namespace ChatSample
{
    public class UserDataManager
    {
        static UserDataManager instance;
        public static UserDataManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new UserDataManager();
                    DateTime startDt = DateTime.Now;
                    while (instance == null)
                    {
                        TimeSpan ts = DateTime.Now - startDt;
                        if (ts.TotalSeconds > 10d)
                        {
                            instance = new UserDataManager();
                            break;
                        }
                    }
                }
                return instance;
            }
            set
            {
                instance = value;
            }
        }
        UserData userData { get; set; }
        public string Udid
        {
            get
            {
                if (userData != null)
                {
                    return userData.Udid;
                }
                userData = Instance.GetItem();
                return userData.Udid;
            }
            set
            {
                if (userData == null)
                {
                    userData = Instance.GetItem();
                }
                userData.Udid = value;
                Instance.UpdateItem(userData);
            }
        }
        public string UserId
        {
            get
            {
                if (userData != null)
                {
                    return userData.UserId;
                    // テストコード
                    //return "2d130968fa52d43a1735ebd129002d2b";
                }
                userData = Instance.GetItem();
                return userData.UserId;
                // テストコード
                //return "2d130968fa52d43a1735ebd129002d2b";
            }
            set
            {
                if (userData == null)
                {
                    userData = Instance.GetItem();
                }
                userData.UserId = value;
                Instance.UpdateItem(userData);
            }
        }


        static readonly object Locker = new object();
        SQLiteConnection Connection { get; set; }
        public UserDataManager()
        {
            Connection = SQLiteService.Connection;
            Connection.CreateTable<UserData>();
            var data = Connection.Table<UserData>().FirstOrDefault();
            // ここで先に空データinsertしてのちの処理ではUpdateだけさせていく
            if (data == null)
            {
                Connection.Insert(new UserData());
            }
        }

        public UserData GetItem()
        {
            lock (Locker)
            {
                var item = Connection.Table<UserData>().FirstOrDefault();
                return item;
            }
        }

        public int UpdateItem(UserData item)
        {
            lock (Locker)
            {
                var id = Connection.Update(item);
                return id;
            }
        }

        public void DeleteItem(int id)
        {
            lock (Locker)
            {
                if (id != 0)
                {
                    Connection.Delete<UserData>(id);
                }
            }
        }
    }
}
