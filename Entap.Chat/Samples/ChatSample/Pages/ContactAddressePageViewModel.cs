using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ChatSample
{
    public class ContactAddressePageViewModel : BindableBase
    {
        public ContactAddressePageViewModel()
        {
            ItemsSource = new ObservableCollection<Item>();
            Task.Run(async () =>
            {
                await GetData();
            });
        }

        async Task GetData()
        {
            var list = new ObservableCollection<Item>();
            var json = await APIManager.PostAsync(APIManager.GetEntapAPI(APIManager.EntapAPIName.GetContactAddresses), new ReqGetUserId());
            var resp = JsonConvert.DeserializeObject<ResqGetContactAddresses>(json);
            if (resp.Status == APIManager.APIStatus.Succeeded)
            {
                foreach (var data in resp.Data.ContactAddresses)
                {
                    var item = new Item
                    {
                        UserId = data.UserId,
                        Name = data.UserName,
                        UserIcon = data.UserIcon
                    };
                    list.Add(item);
                }
            }
            ItemsSource = list;
        }

        public Command DelCmd => new Command(async (obj) =>
        {

        });

        public Command AddCmd => new Command(async () =>
        {
            if (string.IsNullOrEmpty(EditorText))
                return;
            var json = await APIManager.PostAsync(APIManager.GetEntapAPI(APIManager.EntapAPIName.RegistContactAddress), new ReqRegistContactAddress { ContactId=EditorText});
            var resp = JsonConvert.DeserializeObject<ResponseBase>(json);
            if (resp.Status == APIManager.APIStatus.Succeeded)
            {
                await GetData();
            }
        });

        private string editorText;
        public string EditorText
        {
            get
            {
                return editorText;
            }
            set
            {
                if (editorText != value)
                {
                    editorText = value;
                    OnPropertyChanged("EditorText");
                }
            }
        }

        private ObservableCollection<Item> itemsSource;
        public ObservableCollection<Item> ItemsSource
        {
            get
            {
                return itemsSource;
            }
            set
            {
                if (itemsSource != value)
                {
                    itemsSource = value;
                    OnPropertyChanged("ItemsSource");
                }
            }
        }

        public class Item
        {
            public string Name { get; set; }
            public string UserId { get; set; }
            public string UserIcon { get; set; }
        }
    }
}
