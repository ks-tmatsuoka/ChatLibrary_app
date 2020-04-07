using System;
using System.Collections.Generic;
using SQLite;
using Entap.Chat;

namespace ChatSample
{
    public class NotSendMessageManager
    {
        static readonly object Locker = new object();
        SQLiteConnection Connection { get; set; }

        public NotSendMessageManager()
        {
            Connection = SQLiteService.Connection;
            Connection.CreateTable<NotSendMessage>();
        }

        public NotSendMessage GetItem()
        {
            lock (Locker)
            {
                var item = Connection.Table<NotSendMessage>().FirstOrDefault();
                return item;
            }
        }

        public NotSendMessage GetItem(int id)
        {
            lock (Locker)
            {
                var item = Connection.Table<NotSendMessage>().Where(w=>w.Id == id).FirstOrDefault();
                return item;
            }
        }

        public IEnumerable<NotSendMessage> GetItems(int roomId)
        {
            lock (Locker)
            {
                return Connection.Table<NotSendMessage>().Where(w => w.RoomId == roomId).OrderBy(o => o.Id);
            }
        } 

        // アイテムを保存する
        public int SaveItem(NotSendMessage item)
        {
            lock (Locker)
            {
                var id = Connection.Insert(item);
                return id;
            }
        }

        // 指定したIDのアイテムを削除する
        public void DeleteItem(int id)
        {
            lock (Locker)
            {
                if (id != 0)
                {
                    Connection.Delete<NotSendMessage>(id);
                }
            }
        }
    }
}
