using System;
namespace Entap.Chat
{
    /// <summary>
    /// 設定
    /// </summary>
    public class Settings
    {
        static Lazy<Settings> _settings = new Lazy<Settings>(() => new Settings());

        /// <summary>
        /// Current plugin implementation to use
        /// </summary>
        public static Settings Current => _settings.Value;

        public IChatService ChatService;

        public string TimeFormat;
        public string AlreadyReadText;
        public string TodayText;
        public string DateFormat;

        public void Init(IChatService chatService)
        {
            ChatService = chatService;
            TimeFormat = "H:m";
            AlreadyReadText = "既読";
            TodayText = "今日";
            DateFormat = "MM/dd";
        }
    }
}
