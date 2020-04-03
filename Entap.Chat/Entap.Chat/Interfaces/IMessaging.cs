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
        Task<int> SendMessage(MessageBase text);
        Task<string> SelectImage();
        Task<string> TakePicture();
        void UpdateData(ObservableCollection<MessageBase> messageBases);
    }
}
