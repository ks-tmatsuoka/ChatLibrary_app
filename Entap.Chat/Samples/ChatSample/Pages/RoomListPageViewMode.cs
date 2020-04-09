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

            // テストコード
            var page = new ChatPage(1);
            App.Current.MainPage.Navigation.PushAsync(page);
        });
    }
}
