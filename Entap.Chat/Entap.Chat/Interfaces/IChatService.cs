using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms.Internals;

namespace Entap.Chat
{
    [Preserve(AllMembers = true)]
    public interface IChatService
    {
        Task<IEnumerable<MessageBase>> GetMessagesAsync(int roomId, int messageId, int messageDirection);
        Task<bool> SendAlreadyRead(int roomId, int messageId);
        Task<List<ChatMemberBase>> GetRoomMembers(int roomId);
        Task<SendMessageResponseBase> SendMessage(int roomId, MessageBase msg, int notSendMessageId, CancellationTokenSource cts = null);
        void UpdateData(ObservableCollection<MessageBase> messageBases, int roomId);
        void Dispose();
        void AddNotSendMessages(int roomId, ObservableCollection<MessageBase> messageBases);
        void SaveNotSendMessageData(int roomId, MessageBase messageBase, string fileName="");
        void DeleteNotSendMessageData(int id);
        string GetUserId();
        string GetSendImageSaveFolderPath();
        string GetSendVideoSaveFolderPath();
        string GetNotSendMediaSaveFolderPath();
        Task SetRoomMembers(int roomId);
    }
}
