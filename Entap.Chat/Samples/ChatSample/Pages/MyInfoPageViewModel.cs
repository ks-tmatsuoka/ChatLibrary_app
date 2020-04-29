using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ChatSample
{
    public class MyInfoPageViewModel : BindableBase
    {
        public MyInfoPageViewModel()
        {
            Task.Run(async() =>
            {
                var json = await APIManager.PostAsync(APIManager.GetEntapAPI(APIManager.EntapAPIName.GetMyInfomation), new ReqGetUserId());
                var resp = JsonConvert.DeserializeObject<RespGetMyInfomation>(json);
                if (resp.Status == APIManager.APIStatus.Succeeded)
                {
                    ContactId = resp.Data.ContactId;
                    Name = resp.Data.UserName;
                    Icon = resp.Data.UserIcon;
                }
            });
        }

        public Command SelectCmd => new Command(async() =>
        {
            ChangeIcon = await new ChatControlService().SelectImage();
        });

        public Command SaveCmd => new Command(async() =>
        {
            var dic = new Dictionary<string, string>();
            dic["UserId"] = UserDataManager.Instance.UserId;

            if (!string.IsNullOrEmpty(EditorText))
                dic["UserName"] = EditorText;
            else
                dic["UserName"] = Name;

            byte[] bytes = null;
            string name = "";
            string fileType = "";
            var img = ChangeIcon;
            if (string.IsNullOrEmpty(img))
                img = Icon;
            if (!string.IsNullOrEmpty(img))
            {
                // 画像
                bytes = FileManager.ReadBytes(img);
                var extension = System.IO.Path.GetExtension(img);
                name = Guid.NewGuid().ToString() + extension;
                fileType = "UserIcon";
                if (bytes == null || bytes.Length < 1)
                {
                    await App.Current.MainPage.DisplayAlert("この写真は送れません", "", "閉じる");
                    return;
                }

                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(30));
                var json = await APIManager.PostFile(APIManager.GetEntapAPI(APIManager.EntapAPIName.RegistMyInfomation), bytes, name, dic, cts, fileType);
                var resp = JsonConvert.DeserializeObject<ResponseBase>(json);
                if (resp.Status == APIManager.APIStatus.Succeeded)
                {
                    Name = EditorText;
                    EditorText = "";
                    Icon = img;
                    ChangeIcon = "";
                }
            }
        });

        private string contactId;
        public string ContactId
        {
            get
            {
                return contactId;
            }
            set
            {
                if (contactId != value)
                {
                    contactId = value;
                    OnPropertyChanged("ContactId");
                }
            }
        }
        private string name;
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }
        private string icon;
        public string Icon
        {
            get
            {
                return icon;
            }
            set
            {
                if (icon != value)
                {
                    icon = value;
                    OnPropertyChanged("Icon");
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

        private string changeIcon;
        public string ChangeIcon
        {
            get
            {
                return changeIcon;
            }
            set
            {
                if (changeIcon != value)
                {
                    changeIcon = value;
                    OnPropertyChanged("ChangeIcon");
                }
            }
        }
    }
}
