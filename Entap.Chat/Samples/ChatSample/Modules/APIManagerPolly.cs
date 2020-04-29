using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Polly;
using Xamarin.Forms;

namespace ChatSample
{
    public class APIManager
    {
        static HttpClient client;
        static HttpClient infiniteTimeSpanClient;

        // Entap標準
        public class APIStatus
        {
            public const int Succeeded = 0;
            public const int FatalError = 1;
            public const int Retry = 2;
            public const int ValidationError = 3;
            public const int UnderMaintenance = 4;
            public const int AppVersionError = 5;
            public const int DataVersionError = 6;
            public const int SessionTimeOut = 8;
            public const int HttpConnectError = 9;
            public const int RetryFalse = 10;
            public const int Exception = 11;
        }

        // App固有：アプリの更新時、必要に応じてAPIVersionをコントロールする。
        const int MinimunAPIVersion = 1;

        // 簡易対応（直近で取得したAPIVersionをローカルストレージへの保存を推奨）
        static string _APIVersion;
        public static string APIVersion
        {
            get
            {
                if (_APIVersion == null)
                    return MinimunAPIVersion.ToString();

                return _APIVersion;
            }
            set
            {
                _APIVersion = value;
            }
        }

        // App固有
        public const int DefaultTimeoutSecond = 15;
        // Entap標準：リトライ設定
        public const int RetryCount = 3;

        // App固有
        public static string HostName()
        {
            return "https://chat.entap.dev/";
        }
        public static string BaseURL()
        {
            return HostName() + "chat_api/";
        }
        
        public static class EntapAPIName
        {
            public const string AddDevice = "add_device.php";
            public const string GetUserId = "get_userId.php";
            public const string GetRoomList = "get_room_list.php";
            public const string CreateRoom = "create_room.php";
            public const string SendMessage = "send_message.php";
            public const string GetMessages = "get_message.php";
            public const string GetRoomMembers = "get_room_member.php";
            public const string ReadMessage = "read_message.php";
            public const string GetMyInfomation = "get_my_infomation.php";
            public const string RegistMyInfomation = "regist_my_infomation.php";
            public const string GetContactAddresses = "get_contact_addresses.php";
            public const string RegistContactAddress = "regist_contact_address.php";
            public const string DeleteContactAddress = "delete_contact_address.php";
            public const string LeaveRoom = "leave_room.php";
            public const string AddRoomMember = "add_room_member.php";
        }

        /// <summary>
        /// エンタップのAPIのフルパス取得
        /// </summary>
        /// <returns>APIのフルパス</returns>
        /// <param name="apiName">API名</param>
        public static string GetEntapAPI(string apiName)
        {
            return BaseURL() + APIVersion + "/" + apiName;
        }

        public static Dictionary<string, string> ToDictionary(object data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.None);
            var dictionaryData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return dictionaryData;
        }

        async public static Task<string> PostAsync(string apiUrl, object obj, Action errorCallback = null, int timeoutSecond = DefaultTimeoutSecond, bool showError = true)
        {
            var data = ToDictionary(obj);
            return await PostBase(apiUrl, new FormUrlEncodedContent(data), timeoutSecond, showError);
        }

        static bool Validation(HttpResponseMessage msg)
        {
            var comp = new TaskCompletionSource<bool>();
            Task.Run(async() =>
            {
                var responseStr = await msg.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<ResponseBase>(responseStr);

                switch (response.Status)
                {
                    case APIStatus.Succeeded:
                        // 成功
                        comp.SetResult(true);
                        break;

                    case APIStatus.FatalError:
                        // 致命的なエラー：ErrorMessageを表示し、「再試行」・「キャンセル」の選択肢を表示
                        comp.SetResult(false);
                        break;

                    case APIStatus.Retry:
                        // 再試行：一定時間経過後にリクエストを再試行。一定回数繰り返す
                        comp.SetResult(false);
                        break;

                    case APIStatus.ValidationError:
                        // バリデーションエラー：ErrorMessageを表示
                        comp.SetResult(true);
                        break;

                    case APIStatus.UnderMaintenance:
                        // メンテナンス中：ErrorMessageを表示。確認ボタンタップ後はタイトル画面に遷移
                        comp.SetResult(true);
                        break;

                    case APIStatus.SessionTimeOut:
                        // セッションタイムアウト：認証系のAPIリクエストを実行する
                        comp.SetResult(false);
                        break;

                    default:
                        comp.SetResult(false);
                        break;
                }
            });
            return comp.Task.Result;
        }

        private async static Task<string> PostBase(string url, HttpContent content, int timeoutSeconds, bool showError = true)
        {
            Debug.WriteLine("  api manager post  url   : " + url);
            if (client == null)
                client = new HttpClient();
            try
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
                var policyResult = await Policy.HandleResult<HttpResponseMessage>(resp => resp.IsSuccessStatusCode == false || Validation(resp) == false)
                    // 2 秒 * リトライ回数分ずつ待ち時間を増やす
                    .WaitAndRetryAsync(RetryCount, retryAttempt => TimeSpan.FromSeconds(retryAttempt * 2))
                    .ExecuteAsync(async () => await client.PostAsync(url, content, cts.Token));
                if (!policyResult.IsSuccessStatusCode)
                {
                    if (showError)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            App.Current.MainPage.DisplayAlert("通信エラー", policyResult.StatusCode.ToString(), "閉じる");
                        });
                    }
                    return "{\"Status\":" + APIStatus.HttpConnectError + ",\"Message\": \"\",\"Data\": {}}";
                }

                var responseStr = await policyResult.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<ResponseBase>(responseStr);
                if (!showError)
                    return responseStr;

                switch (response.Status)
                {
                    case APIStatus.Succeeded:
                        // 成功
                        return responseStr;

                    case APIStatus.FatalError:
                        // 致命的なエラー：ErrorMessageを表示し、「再試行」・「キャンセル」の選択肢を表示
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            App.Current.MainPage.DisplayAlert("エラー", response.Message, "閉じる");
                        });
                        return responseStr;

                    case APIStatus.ValidationError:
                        // バリデーションエラー：ErrorMessageを表示
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            App.Current.MainPage.DisplayAlert("エラー", response.Message, "閉じる");
                        });
                        return responseStr;

                    case APIStatus.UnderMaintenance:
                        // メンテナンス中：ErrorMessageを表示。確認ボタンタップ後はタイトル画面に遷移
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            App.Current.MainPage.DisplayAlert("メンテナンス中", response.Message, "閉じる");
                        });
                        return responseStr;

                    case APIStatus.SessionTimeOut:
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            App.Current.MainPage.DisplayAlert("エラー", "タイムアウトしました", "閉じる");
                        });
                        return responseStr;
                }
                return responseStr;
            }
            catch (Exception ex)
            {
                Debug.WriteLine("API_PostError : " + ex.Message);
                //if (showError)
                //{
                //    Device.BeginInvokeOnMainThread(() =>
                //    {
                //        App.Current.MainPage.DisplayAlert("エラー", "Exception:" + ex.Message, "閉じる");
                //    });
                //}
                return "{\"Status\":" + APIStatus.Exception + ",\"Message\": \"" + ex.Message.Replace("\"", " ").Replace(":", " ") + "\",\"Data\": {}}";
            }
        }


        public static async Task<string> PostFile(string url, byte[] fileData, string fileName, Dictionary<string, string> DictionaryData, CancellationTokenSource cancellationTokenSource, string fileType = "file", ProgressDelegate progressDelegate  = null, Action errorCallback = null)
        {
            var content = new MultipartFormDataContent();
            foreach (KeyValuePair<string, string> pair in DictionaryData)
            {
                content.Add(new StringContent(pair.Value), pair.Key);
            }

            ProgressStreamContent progressContent = null;
            if (fileData != null && fileData.Length > 0)
            {
                
                if (progressDelegate != null)
                {
                    progressContent = new ProgressStreamContent(new MemoryStream(fileData));
                    progressContent.Progress = progressDelegate;
                    content.Add(progressContent, fileType, fileName);
                }
                else
                {
                    content.Add(new StreamContent(new MemoryStream(fileData)), fileType, fileName);   
                }
            }

            bool isLoopRunning = true;
            if (infiniteTimeSpanClient == null)
            {
                infiniteTimeSpanClient = new HttpClient() {Timeout= Timeout.InfiniteTimeSpan };
            }
            try
            {
                Task.Run(() =>
                {
                    while (isLoopRunning)
                    {
                        if (cancellationTokenSource.IsCancellationRequested)
                        {
                            if (progressContent != null)
                            {
                                progressContent.Dispose();
                            }
                        }
                    }
                    
                });
                var result =  await infiniteTimeSpanClient.PostAsync(url, content, cancellationTokenSource.Token);
                isLoopRunning = false;
                if (!result.IsSuccessStatusCode)
                {
                    return "{\"Status\":" + APIStatus.HttpConnectError + ",\"Message\": \"\",\"Data\": {}}";
                }

                var responseStr = await result.Content.ReadAsStringAsync();
                var response = JsonConvert.DeserializeObject<ResponseBase>(responseStr);

                switch (response.Status)
                {
                    case APIStatus.Succeeded:
                        // 成功
                        return responseStr;

                    case APIStatus.FatalError:
                        // 致命的なエラー：ErrorMessageを表示し、「再試行」・「キャンセル」の選択肢を表示
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            App.Current.MainPage.DisplayAlert("エラー", response.Message, "閉じる");
                        });
                        return responseStr;

                    case APIStatus.ValidationError:
                        // バリデーションエラー：ErrorMessageを表示
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            App.Current.MainPage.DisplayAlert("エラー", response.Message, "閉じる");
                        });
                        return responseStr;

                    case APIStatus.UnderMaintenance:
                        // メンテナンス中：ErrorMessageを表示。確認ボタンタップ後はタイトル画面に遷移
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            App.Current.MainPage.DisplayAlert("メンテナンス中", response.Message, "閉じる");
                        });
                        return responseStr;

                    case APIStatus.SessionTimeOut:
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            App.Current.MainPage.DisplayAlert("エラー", "タイムアウトしました", "閉じる");
                        });
                        return responseStr;
                }
                return responseStr;
            }
            catch (Exception ex)
            {
                isLoopRunning = false;
                Debug.WriteLine("API_PostError : " + ex.Message);
                return "{\"Status\":" + APIStatus.Exception + ",\"Message\": \"" + ex.Message.Replace("\"", " ").Replace(":", " ") + "\",\"Data\": {}}";
            }
        }
    }
}
