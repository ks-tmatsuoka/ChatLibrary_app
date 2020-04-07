using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Entap.Chat
{
    [Preserve(AllMembers = true)]
    public interface IMessaging
    {
        Task<IEnumerable<MessageBase>> GetMessagesAsync(int index, int count);
        Task<IEnumerable<MessageBase>> GetNewMessagesAsync(int index, int count);
        Task<int> SendMessage(MessageBase text);
        Task<int> SendAlreadyRead(int msgId);
        Task<string> SelectImage();
        Task<string> TakePicture();
        void UpdateData(ObservableCollection<MessageBase> messageBases);
        void AddNotSendMessages(int roomId, ObservableCollection<MessageBase> messageBases);
        void SaveNotSendMessageData(int roomId, MessageBase messageBase);
        void DeleteNotSendMessageData(int id);

    }
}
