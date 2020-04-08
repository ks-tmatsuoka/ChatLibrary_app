using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace ChatSample
{
    public class RoomListPageViewMode : BindableBase
    {
        public RoomListPageViewMode()
        {
            ItemsSource = new ObservableCollection<Room>();
            Task.Run(async() =>
            {
                var json = await APIManager.PostAsync(APIManager.GetEntapAPI(APIManager.EntapAPIName.GetRoomList), new ReqGetRoomList());
                var respGetRooms = JsonConvert.DeserializeObject<RespGetRoomList>(json);
                if (respGetRooms.Status == APIManager.APIStatus.Succeeded)
                {
                    if (respGetRooms.Data.Rooms.Count < 1)
                    {
                        //サービス管理者とのルーム作成
                        var data = new ReqCreateRoomData()
                        {
                            UserId = UserDataManager.Instance.UserId,
                            RoomType = 1
                        };
                        var reqJson = JsonConvert.SerializeObject(data);
                        json = await APIManager.PostAsync(APIManager.GetEntapAPI(APIManager.EntapAPIName.CreateRoom), new ReqCreateRoom { Data=reqJson});
                        var respCreateRoom = JsonConvert.DeserializeObject<RespCreateRoom>(json);
                        if (respCreateRoom.Status == APIManager.APIStatus.Succeeded)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                ItemsSource.Add(respCreateRoom.Data);
                            });
                        }
                    }
                    else
                    {
                        foreach(var room in respGetRooms.Data.Rooms)
                        {
                            Device.BeginInvokeOnMainThread(() =>
                            {
                                ItemsSource.Add(room);
                            });
                        }
                    }
                }

                //var reqGetUserId = new ReqGetUserId();
                //if (string.IsNullOrEmpty(reqGetUserId.UserId))
                //{
                //    var json = await APIManager.PostAsync(APIManager.GetEntapAPI(APIManager.EntapAPIName.GetUserId), reqGetUserId);
                //    var resp = JsonConvert.DeserializeObject<RespUserId>(json);
                //    if (resp.Status == APIManager.APIStatus.Succeeded)
                //    {
                //        if (UserDataManager.Instance.UserId != resp.Data.UserId)
                //            UserDataManager.Instance.UserId = resp.Data.UserId;
                //    }
                //}
            });
            
        }

        private ObservableCollection<Room> itemsSource;
        public ObservableCollection<Room> ItemsSource
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

        public Command ItemTappedCmd => new Command((obj) =>
        {
            if (!(obj is ItemTappedEventArgs itemTappedEventArgs)) return;
            if (!(itemTappedEventArgs.Item is Room model)) return;

            var page = new ChatPage();
            App.Current.MainPage.Navigation.PushAsync(page);

            //ProcessManager.CanInvoke(async () =>
            //{
            //    var entry = model.RightImageTapCmdParameter as RespGetReportEntries.Entry;
            //    if (model.IsFolder)
            //    {
            //        await PageManager.PushAsync<DocumentsAndPhotoPage>(new DocumentsAndPhotoPageViewModel(model.TopText, permission, entry.EntryId, project, apiName, false, isMove, moveEntry));
            //    }
            //    else
            //    {
            //        await PageManager.PushAsync<DisplayFilePage>(new DisplayFilePageViewModel(model.FilesUrl, false));
            //    }
            //    ProcessManager.OnComplete();
            //});
        });
    }
}
