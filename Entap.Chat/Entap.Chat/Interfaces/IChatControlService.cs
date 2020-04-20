using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Entap.Chat
{
    public interface IChatControlService
    {
        Task<SendMessageResponseBase> SendMessage(int roomId, MessageBase msg, int notSendMessageId);
        Task<string> SelectImage();
        Task<string> TakePicture();
        Task ImageShare(string imagePath);
        string GetNotSendImageSaveFolderPath();
        void MoveImagePreviewPage(string imageUrl);
        Task<IEnumerable<MessageBase>> BottomControllerMenuExecute(int notSendMessageId, int type, int roomId, ChatListView chatListView);
    }
}
