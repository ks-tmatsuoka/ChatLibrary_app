using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ChatSample
{
    public class ContactAddressePageViewModel : BindableBase
    {
        public ContactAddressePageViewModel()
        {
            ItemsSource = new ObservableCollection<ContactItem>();
            Task.Run(async () =>
            {
                await GetData();
            });
        }

        async Task GetData()
        {
            var list = new ObservableCollection<ContactItem>();
            var json = await APIManager.PostAsync(APIManager.GetEntapAPI(APIManager.EntapAPIName.GetContactAddresses), new ReqGetUserId());
            var resp = JsonConvert.DeserializeObject<ResqGetContactAddresses>(json);
            if (resp.Status == APIManager.APIStatus.Succeeded)
            {
                foreach (var data in resp.Data.ContactAddresses)
                {
                    var item = new ContactItem
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
            var delUseId = obj as string;
            var req = new DeleteContactAddress
            {
                DeleteUseId = delUseId
            };
            var json = await APIManager.PostAsync(APIManager.GetEntapAPI(APIManager.EntapAPIName.DeleteContactAddress), req);
            var resp = JsonConvert.DeserializeObject<ResponseBase>(json);
            if (resp.Status == APIManager.APIStatus.Succeeded)
            {
                await GetData();
            }
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

        private Command itemTappedCmd;
        public Command ItemTappedCmd
        {
            get
            {
                return itemTappedCmd;
            }
            set
            {
                if (itemTappedCmd != value)
                {
                    itemTappedCmd = value;
                    OnPropertyChanged("ItemTappedCmd");
                }
            }
        }

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

        private ObservableCollection<ContactItem> itemsSource;
        public ObservableCollection<ContactItem> ItemsSource
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

        private bool contactListPageFlag;
        public bool ContactListPageFlag
        {
            get
            {
                return contactListPageFlag;
            }
            set
            {
                if (contactListPageFlag != value)
                {
                    contactListPageFlag = value;
                    OnPropertyChanged("ContactListPageFlag");
                }
            }
        }
    }

    public class ContactItem
    {
        public string Name { get; set; }
        public string UserId { get; set; }
        public string UserIcon { get; set; }
    }
}
