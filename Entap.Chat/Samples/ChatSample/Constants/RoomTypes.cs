using System;
namespace ChatSample
{
    public class RoomTypes
    {
        // 管理者との1対1のルーム
        public const int AdminDirect = 1;

        // 管理者との複数人のルーム
        public const int AdminGroup = 2;

        // 管理者以外の人との1対1のルーム
        public const int UserDirect = 3;

        // 管理者以外の人との複数人のルーム
        public const int UserGroup = 4;
    }
}
