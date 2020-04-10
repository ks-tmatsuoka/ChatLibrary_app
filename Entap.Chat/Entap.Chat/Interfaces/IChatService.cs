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
        Task<IEnumerable<MessageBase>> GetMessagesAsync(int roomId, int messageId, int messageDirection, List<ChatMemberBase> members);
        Task<int> SendMessage(int roomId, MessageBase msg);
        Task<bool> SendAlreadyRead(int roomId, int messageId);
        Task<string> SelectImage();
        Task<string> TakePicture();
        Task<List<ChatMemberBase>> GetRoomMembers(int roomId);
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
