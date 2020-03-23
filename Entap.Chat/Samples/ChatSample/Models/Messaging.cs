using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Entap.Chat;
using Xamarin.Forms;

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



        public Task<IEnumerable<SekiTestModel>> GetMessagesAsync(ObservableCollection<SekiTestModel> itemsSorce)
        {
            var messages = new List<SekiTestModel>();
            var firstMsg = itemsSorce.FirstOrDefault();
            int index = firstMsg.Id;
            if (index <= 0)
                return Task.FromResult<IEnumerable<SekiTestModel>>(messages);

            for (int i = index - 1; i > index - 21; i--)
            {
                if (i <= 0)
                    break;
                if (i % 3 == 0)
                {
                    if (i % 2 == 0)
                        messages.Add(new SekiTestModel { Id = i, Text = "" + Environment.NewLine + i.ToString() + Environment.NewLine + "", BgColor = Color.Aqua });
                    else
                        messages.Add(new SekiTestModel { Id = i, Text = "" + Environment.NewLine + "" + Environment.NewLine + i.ToString() + Environment.NewLine + "" + Environment.NewLine + "", BgColor = Color.Yellow });
                }
                else
                    messages.Add(new SekiTestModel { Id = i, Text = i.ToString() });
            }

            return Task.FromResult<IEnumerable<SekiTestModel>>(messages);
        }

        public Task<IEnumerable<SekiTestModel>> GetNewMessagesAsync(ObservableCollection<SekiTestModel> itemsSorce)
        {
            var messages = new List<SekiTestModel>();
            var lastMsg = itemsSorce.LastOrDefault();
            int index = lastMsg.Id;

            if (index >= 300)
                return Task.FromResult<IEnumerable<SekiTestModel>>(messages);

            for (int i = index + 1; i <= index + 20; i++)
            {
                if (i % 3 == 0)
                {
                    if (i % 2 == 0)
                        messages.Add(new SekiTestModel { Id = i, Text = "" + Environment.NewLine + i.ToString() + Environment.NewLine + "", BgColor = Color.Aqua });
                    else
                        messages.Add(new SekiTestModel { Id = i, Text = "" + Environment.NewLine + "" + Environment.NewLine + i.ToString() + Environment.NewLine + "" + Environment.NewLine + "", BgColor = Color.Yellow });
                }
                else
                    messages.Add(new SekiTestModel { Id = i, Text = i.ToString() });
            }
            return Task.FromResult<IEnumerable<SekiTestModel>>(messages);
        }
    }
}
