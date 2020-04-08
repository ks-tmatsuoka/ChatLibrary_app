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

        public void Init(IChatService chatService)
        {
            ChatService = chatService;
        }
    }
}
