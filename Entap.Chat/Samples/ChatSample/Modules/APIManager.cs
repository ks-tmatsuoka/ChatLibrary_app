using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xamarin.Forms;
using Newtonsoft.Json;
using System.Linq;
using System.Threading;

namespace ChatSample
{
    public class APIManager
    {
        static HttpClient client;

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
        public const int RetryInterval = 3000;

        // App固有
        public static string HostName()
        {
            return "https://chat.entap.dev/";
        }
        public static string BaseURL()
        {
            return HostName() + "api/";
        }
        // App固有
        //public static string WebBaseURL()
        //{
        //    return HostName() + "app/";
        //}

        public static class EntapAPIName
        {
            public const string AddDevice = "add_device.php";
            public const string GetUserId = "get_userId.php";
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

        //public static string OrderUrl = WebBaseURL() + EntapAPIName.Order;
        //public static string HisotryUrl = WebBaseURL() + "history/index.php";
        //public static string NewsUrl = WebBaseURL() + "news/index.php";
        //public static string AboutUrl = WebBaseURL() + "read/about.html";
        //public static string HowToUrl = WebBaseURL() + "read/howto.html";
        //public static string UserpolicyUrl = WebBaseURL() + "read/userpolicy.html";
        //public static string PrivacypolicyUrl = WebBaseURL() + "read/privacypolicy.html";
        //public static string LicenseUrl = WebBaseURL() + "read/license.html";
        //public static string NotationUrl = WebBaseURL() + "read/notation.html";

        public static class OutsideAPI
        {

        }


        public static Dictionary<string, string> ToDictionary(object data)
        {
            var json = JsonConvert.SerializeObject(data, Formatting.None);
            var dictionaryData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return dictionaryData;
        }

        async public static Task<string> GET(string apiUrl, object obj)
        {
            Dictionary<string, string> dictionaryData = null;
            if (obj != null)
            {
                dictionaryData = ToDictionary(obj);
            }

            MakeGetParam(ref apiUrl, dictionaryData);

            if (client == null)
                client = new HttpClient();
            try
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(DefaultTimeoutSecond));
                var posttask = await client.GetAsync(apiUrl, cts.Token);
                var responsetask = await posttask.Content.ReadAsStringAsync();
                return responsetask;
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
                return null;
            }

        }

        static void MakeGetParam(ref string apiUrl, Dictionary<string, string> dictionary)
        {
            if (dictionary != null)
            {
                var parm = "?";
                int loop = 1;
                foreach (KeyValuePair<string, string> pair in dictionary)
                {
                    var str = pair.Key + "=" + pair.Value;
                    parm += str;
                    if (loop < dictionary.Count)
                    {
                        parm += "&";
                    }
                    loop++;
                }
                apiUrl += parm;
            }
        }
        //async public static Task<string> PostArray(string apiUrl, List<KeyValuePair<string, string>> ary = null, Action errorCallback = null, int timeoutSecond = DefaultTimeoutSecond)
        //{
        //    FormUrlEncodedContent postcontent = null;
        //    if (ary != null)
        //    {
        //        postcontent = new FormUrlEncodedContent(ary.ToArray());
        //    }
        //    return await PostBase(apiUrl, postcontent, timeoutSecond, true);
        //}

        async public static Task<string> PostAsync(string apiUrl, object obj, Action errorCallback = null, int timeoutSecond = DefaultTimeoutSecond, bool showError = true)
        {
            var data = ToDictionary(obj);
            return await PostBase(apiUrl, new FormUrlEncodedContent(data), timeoutSecond, showError);
        }

        private async static Task<string> PostBase(string url, HttpContent content, int timeoutSeconds, bool showError = true)
        {
            Debug.WriteLine("  api manager post  url   : " + url);

            if (client == null)
                client = new HttpClient();
            //client.Timeout = TimeSpan.FromSeconds(timeoutSeconds);
            try
            {
                var cts = new CancellationTokenSource();
                cts.CancelAfter(TimeSpan.FromSeconds(timeoutSeconds));
                for (int i = 0; i < RetryCount; i++)
                {
                    var posttask = await client.PostAsync(url, content, cts.Token);
                    if (!posttask.IsSuccessStatusCode && showError)
                    {
                        await App.Current.MainPage.DisplayAlert("通信エラー", "通信に失敗しました", "閉じる");
                        return "{\"status\":" + APIStatus.HttpConnectError + ", \"message\":[], \"data\":{ }}";
                    }
                    var responseStr = await posttask.Content.ReadAsStringAsync();
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
                            var retry = await App.Current.MainPage.DisplayAlert("エラー", response.Message, "再試行", "キャンセル");
                            //var retry = await App.Current.MainPage.DisplayAlert("エラー", response.Message, "再試行", "キャンセル");
                            if (!retry)
                                return responseStr;

                            break;
                        case APIStatus.Retry:
                            // 再試行：一定時間経過後にリクエストを再試行。一定回数繰り返す
                            await Task.Delay(RetryInterval);

                            break;
                        case APIStatus.ValidationError:
                            // バリデーションエラー：ErrorMessageを表示
                            await App.Current.MainPage.DisplayAlert("エラー", response.Message, "確認");
                            //await App.Current.MainPage.DisplayAlert("エラー", response.Message, "確認");
                            return responseStr;

                        case APIStatus.UnderMaintenance:
                            // メンテナンス中：ErrorMessageを表示。確認ボタンタップ後はタイトル画面に遷移
                            await App.Current.MainPage.DisplayAlert("メンテナンス中", response.Message, "確認");
                            //await App.Current.MainPage.DisplayAlert("メンテナンス中", response.Message, "確認");
                            //App.Current.MainPage = new MaintenancePage();
                            return responseStr;
                        
                        case APIStatus.SessionTimeOut:
                            // セッションタイムアウト：認証系のAPIリクエストを実行する

                            return responseStr;
                    }
                }
                if (showError)
                {
                    await App.Current.MainPage.DisplayAlert("エラー", "通信に失敗しました", "閉じる");
                }
                return "{\"status\":" + APIStatus.RetryFalse + ", \"message\":[\"リトライ失敗\"], \"data\":{ }}";
            }
            catch (Exception ex)
            {
                Debug.WriteLine("API_PostError : " + ex.Message);
                //if (showError)
                //{
                //    ProcessManager.DisplayApiErrorCanInvoke(() =>
                //    {
                //        Device.BeginInvokeOnMainThread(async () =>
                //        {
                //            await App.Current.MainPage.DisplayAlert("エラー", ex.Message, "閉じる");
                //            ProcessManager.DisplayApiErrorOnComplete();
                //        });
                //    });
                    
                //}
                return "{\"status\":" + APIStatus.Exception + ", \"message\":[\"" + ex.Message.Replace("\"", " ").Replace(":", " ") + "\"]}";
            }
        }


        public static async Task<string> PostFile(string url, byte[] fileData, string fileName, Dictionary<string, string> DictionaryData, string fileType = "file", Action errorCallback = null, int timeoutSecond = 30)
        {
            var content = new MultipartFormDataContent();

            foreach (KeyValuePair<string, string> pair in DictionaryData)
            {
                content.Add(new StringContent(pair.Value), pair.Key);
            }
            if (fileData != null && fileData.Length > 0)
                content.Add(new StreamContent(new MemoryStream(fileData)), fileType, fileName);
            return await PostBase(url, content, timeoutSecond);
        }
    }
}
