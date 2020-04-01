using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Entap.Chat
{
    public interface IMessaging
    {
        Task<IEnumerable<MessageBase>> GetMessagesAsync(int index, int count);
        Task<IEnumerable<MessageBase>> GetNewMessagesAsync(int index, int count);
        Task<int> SendTextMessage(string text);
        Task<int> SendImage(byte[] imageData);
        Task<string> SelectImage();
        Task<string> TakePicture();
        void UpdateData(ObservableCollection<MessageBase> messageBases);
    }
}
