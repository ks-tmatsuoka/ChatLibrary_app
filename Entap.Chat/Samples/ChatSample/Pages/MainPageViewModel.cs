using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ChatSample
{
    public class MainPageViewModel : PageViewModelBase
    {
        public MainPageViewModel()
        {
            Task.Run(async () =>
            {
                var reqAddDevice = new ReqAddDevice();
                if (string.IsNullOrEmpty(reqAddDevice.Udid))
                {
                    var json = await APIManager.PostAsync(APIManager.GetEntapAPI(APIManager.EntapAPIName.AddDevice), reqAddDevice);
                    var resp = JsonConvert.DeserializeObject<RespAddDevice>(json);
                    if (resp.Status == APIManager.APIStatus.Succeeded)
                    {
                        if (UserDataManager.Instance.Udid != resp.Data.Udid)
                            UserDataManager.Instance.Udid = resp.Data.Udid;
                    }
                }

                var reqGetUserId = new ReqGetUserId();
                if (string.IsNullOrEmpty(reqGetUserId.UserId))
                {
                    var json = await APIManager.PostAsync(APIManager.GetEntapAPI(APIManager.EntapAPIName.GetUserId), reqGetUserId);
                    var resp = JsonConvert.DeserializeObject<RespUserId>(json);
                    if (resp.Status == APIManager.APIStatus.Succeeded)
                    {
                        if (UserDataManager.Instance.UserId != resp.Data.UserId)
                            UserDataManager.Instance.UserId = resp.Data.UserId;
                    }
                }
            });
        }
    }
}
