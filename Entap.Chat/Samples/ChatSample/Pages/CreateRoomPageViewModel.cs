using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Xamarin.Forms;
namespace ChatSample
{
    public class CreateRoomPageViewModel : BindableBase
    {
        int _type;
        int _roomId;
        public CreateRoomPageViewModel(int type, int roomId)
        {
            _roomId = roomId;
            _type = type;
            if (_type == 1)
            {
                RoomNameVisible = true;
                ToolberText = "作成";
            }
            else
            {
                RoomNameVisible = false;
                ToolberText = "招待";
            }
        }
        public List<string> InvitationUserIds = new List<string>();
        public bool RoomNameVisible { get; set; }
        public string ToolberText { get; set; }

        private string invitationUsers;
        public string InvitationUsers
        {
            get
            {
                return invitationUsers;
            }
            set
            {
                if (invitationUsers != value)
                {
                    invitationUsers = value;
                    OnPropertyChanged("InvitationUsers");
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
        
        public Command AddCmd => new Command(() =>
        {
            var page = new ContactAddressePage();
            App.Current.MainPage.Navigation.PushAsync(page);
            var vm = ((ContactAddressePageViewModel)page.BindingContext);
            vm.ContactListPageFlag = false;
            vm.ItemTappedCmd = new Command((obj) =>
            {
                if (!(obj is ItemTappedEventArgs itemTappedEventArgs)) return;
                if (!(itemTappedEventArgs.Item is ContactItem selectItem)) return;

                if (vm.ContactListPageFlag)
                    return;
                if (InvitationUserIds.Contains(selectItem.UserId))
                {
                    App.Current.MainPage.Navigation.PopAsync();
                    return;
                }

                InvitationUsers += selectItem.Name + System.Environment.NewLine;
                InvitationUserIds.Add(selectItem.UserId);
                App.Current.MainPage.Navigation.PopAsync();
            });

        });

        public Command ToolbarCmd => new Command(async() =>
        {
            if (InvitationUserIds.Count < 1)
            {
                await App.Current.MainPage.DisplayAlert("", "招待者を選択してください", "閉じる");
                return;
            }
            if (_type == 1)
            {
                if (string.IsNullOrEmpty(EditorText))
                {
                    await App.Current.MainPage.DisplayAlert("", "ルーム名を入力してください", "閉じる");
                    return;
                }

                // RoomType3:管理者以外の人との1対1のルーム  4:管理者以外の人との複数人のルーム
                var data = new ReqCreateRoomData()
                {
                    UserId = UserDataManager.Instance.UserId,
                    RoomName = EditorText,
                    RoomType = RoomTypes.UserDirect,
                    InvitationUserIds = InvitationUserIds
                };
                if (data.InvitationUserIds.Count > 1)
                    data.RoomType = RoomTypes.UserGroup;

                var reqJson = JsonConvert.SerializeObject(data);
                var json = await APIManager.PostAsync(APIManager.GetEntapAPI(APIManager.EntapAPIName.CreateRoom), new ReqCreateRoom { Data = reqJson });
                var respCreateRoom = JsonConvert.DeserializeObject<RespCreateRoom>(json);
                if (respCreateRoom.Status == APIManager.APIStatus.Succeeded)
                {
                    await App.Current.MainPage.DisplayAlert("", "ルームを作成しました", "閉じる");
                    await App.Current.MainPage.Navigation.PopAsync();
                }
            }
            else
            {
                var data = new ReqAddRoommemberData()
                {
                    UserId = UserDataManager.Instance.UserId,
                    RoomId = _roomId,
                    InvitationUserIds = InvitationUserIds
                };
                var reqJson = JsonConvert.SerializeObject(data);
                var json = await APIManager.PostAsync(APIManager.GetEntapAPI(APIManager.EntapAPIName.AddRoomMember), new ReqAddRoommember { Data = reqJson });
                var resp = JsonConvert.DeserializeObject<ResponseBase>(json);
                if (resp.Status == APIManager.APIStatus.Succeeded)
                {
                    await App.Current.MainPage.DisplayAlert("", "ルームに招待しました", "閉じる");
                    await App.Current.MainPage.Navigation.PopAsync();
                }
            }
        });
    }
}
