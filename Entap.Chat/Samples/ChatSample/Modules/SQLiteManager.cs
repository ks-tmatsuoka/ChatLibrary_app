using System;
using System.IO;
using SQLite;

namespace ChatSample
{
    public class SQLiteManager
    {
        public SQLiteManager()
        {
        }

        static SQLiteManager instance;
        public static SQLiteManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SQLiteManager();
                }
                return instance;
            }
            set
            {
                instance = value;
            }
        }

        SQLiteConnection conn { get; set; }

        const string sqliteFilename = "EntapChat.db3";


        public SQLiteConnection GetConnection()
        {
            if (conn == null)
            {
                conn = new SQLiteConnection(GetSqliteFilePath(), SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
                conn.BusyTimeout = TimeSpan.FromSeconds(1);
            }
            return conn;
        }
        public byte[] GetSqliteFile()
        {
            var fileByte = System.IO.File.ReadAllBytes(GetSqliteFilePath());
            return fileByte;
        }

        string GetSqliteFilePath()
        {
            var documentsPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData);
            var path = Path.Combine(documentsPath, sqliteFilename);
            return path;
        }

        public string GetSqliteFileName()
        {
            return sqliteFilename;
        }
    }
}
