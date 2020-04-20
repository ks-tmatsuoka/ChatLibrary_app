using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entap.Chat;
using Newtonsoft.Json;
using System.Linq;

namespace ChatSample
{
    public class CustomChatPageViewModel : PageViewModelBase
    {
        int roomId;
        public CustomChatPageViewModel()
        {
            roomId = InitRoomData();
        }

        int InitRoomData()
        {
            var comp = new TaskCompletionSource<int>();
            Task.Run(async () =>
            {
                var json = await APIManager.PostAsync(APIManager.GetEntapAPI(APIManager.EntapAPIName.GetRoomList), new ReqGetRoomList());
                var respGetRooms = JsonConvert.DeserializeObject<RespGetRoomList>(json);
                if (respGetRooms.Status == APIManager.APIStatus.Succeeded)
                {
                    var chatService = new ChatService();
                    if (respGetRooms.Data.Rooms.Count < 1)
                    {
                        //サービス管理者とのルーム作成
                        var data = new ReqCreateRoomData()
                        {
                            UserId = UserDataManager.Instance.UserId,
                            RoomType = 1
                        };
                        var reqJson = JsonConvert.SerializeObject(data);
                        json = await APIManager.PostAsync(APIManager.GetEntapAPI(APIManager.EntapAPIName.CreateRoom), new ReqCreateRoom { Data = reqJson });
                        var respCreateRoom = JsonConvert.DeserializeObject<RespCreateRoom>(json);
                        if (respCreateRoom.Status == APIManager.APIStatus.Succeeded)
                        {
                            comp.SetResult(respCreateRoom.Data.RoomId);
                        }
                        else
                        {
                            comp.SetResult(0);
                        }
                    }
                    else
                    {
                        var room = respGetRooms.Data.Rooms.Where(w => w.RoomType == 1).LastOrDefault();
                        if (room != null)
                            comp.SetResult(room.RoomId);
                        else
                            comp.SetResult(0);
                    }
                }
                else
                {
                    comp.SetResult(0);
                }
            });

            return comp.Task.Result;
        }
    }
}
