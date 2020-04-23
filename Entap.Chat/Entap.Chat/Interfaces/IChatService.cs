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
        Task<bool> SendAlreadyRead(int roomId, int messageId);
        Task<List<ChatMemberBase>> GetRoomMembers(int roomId);
        void UpdateData(ObservableCollection<MessageBase> messageBases, int roomId, List<ChatMemberBase> members);
        void Dispose();
        void AddNotSendMessages(int roomId, ObservableCollection<MessageBase> messageBases);
        void SaveNotSendMessageData(int roomId, MessageBase messageBase, string fileName="");
        void DeleteNotSendMessageData(int id);
        string GetUserId();
        string GetSendImageSaveFolderPath();
        string GetSendVideoSaveFolderPath();
        string GetNotSendMediaSaveFolderPath();
    }
}
