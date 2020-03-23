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

        Task<IEnumerable<SekiTestModel>> GetMessagesAsync(ObservableCollection<SekiTestModel> itemsSorce);
        Task<IEnumerable<SekiTestModel>> GetNewMessagesAsync(ObservableCollection<SekiTestModel> itemsSorce);
    }
}
