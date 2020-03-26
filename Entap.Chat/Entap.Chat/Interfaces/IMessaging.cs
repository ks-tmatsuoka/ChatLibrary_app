using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Entap.Chat
{
    public interface IMessaging
    {
        Task<IEnumerable<MessageBase>> GetMessagesAsync(int index, int count);
        Task<IEnumerable<MessageBase>> GetNewMessagesAsync(int index, int count);
        Task<bool> SendTextMessage(string text);
        Task<bool> SendImage(byte[] imageData);
        Task<string> SelectImage();
        Task<string> TakePicture();
    }
}
