using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Entap.Chat
{
    [Preserve(AllMembers = true)]
    public interface IChatService
    {
        Task<IEnumerable<MessageBase>> GetMessagesAsync(int roomId, int messageId);
        Task<IEnumerable<MessageBase>> GetNewMessagesAsync(int roomId, int messageId);
        Task<int> SendMessage(int roomId, MessageBase msg);
        Task<int> SendAlreadyRead(int msgId);
        Task<string> SelectImage();
        Task<string> TakePicture();
        void UpdateData(ObservableCollection<MessageBase> messageBases);
        void AddNotSendMessages(int roomId, ObservableCollection<MessageBase> messageBases);
        void SaveNotSendMessageData(int roomId, MessageBase messageBase);
        void DeleteNotSendMessageData(int id);

        Task ImageShare(string imagePath);
        Task ImageDownload(string imageUrl);
        string GetSendImageSaveFolderPath();
        string GetNotSendImageSaveFolderPath();
        string GetUserId();
    }
}
