using System;
using System.IO;
using SQLite;

namespace ChatSample
{
    public class SQLiteService
    {
        public static string DatabasePath =>
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                "EntapChat.db3");

        static SQLiteAsyncConnection _asyncConnection;
        public static SQLiteAsyncConnection AsyncConnection = _asyncConnection ?? (_asyncConnection = GetAsyncConnection());

        public static SQLiteAsyncConnection GetAsyncConnection()
        {
            return new SQLiteAsyncConnection(DatabasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
        }

        static SQLiteConnection _connection;
        public static SQLiteConnection Connection = _connection ?? (_connection = GetConnection());

        public static SQLiteConnection GetConnection()
        {
            var conn = new SQLiteConnection(DatabasePath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.FullMutex | SQLiteOpenFlags.Create);
            conn.BusyTimeout = TimeSpan.FromSeconds(1);
            return conn;
        }
    }
}
