using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entap.Chat;
namespace ChatSample
{
    public class Messaging : IMessaging
    {
        public Task<IEnumerable<MessageBase>> GetMessagesAsync(int id, int count)
        {
            if (id == 0) return null;
            if (id < 0)
                id = 100;

            var messages = new List<MessageBase>();
            for (int i = 0; i < count; i++)
            {
                if (id - i < 0) break;

                var mod = (id - i) % 2;
                if (mod == 0)
                    messages.Add(new TextMessage { Id = id - i });
                else if (mod == 1)
                    messages.Add(new ImageMessage { Id = id - i });
            }
            messages.Reverse();
            return Task.FromResult<IEnumerable<MessageBase>>(messages);
        }

        public Task<IEnumerable<MessageBase>> GetNewMessagesAsync(int id, int count)
        {
            var messages = new List<MessageBase>();
            for (int i = 0; i < count; i++)
            {
                if (id - i < 0) break;

                var mod = (id - i) % 2;
                if (mod == 0)
                    messages.Add(new TextMessage { Id = id + i });
                else if (mod == 1)
                    messages.Add(new ImageMessage { Id = id + i });
            }
            return Task.FromResult<IEnumerable<MessageBase>>(messages);
        }
    }
}
