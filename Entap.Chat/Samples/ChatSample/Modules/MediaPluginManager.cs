using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace ChatSample
{
    public class MediaPluginManager
    {
        public MediaPluginManager()
        {
            CrossMedia.Current.Initialize();
            ClearMediaFolder();
        }

        const string MediaFolder = "temp";

        public string GetPersonalFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        public string GetMediaFolderPath()
        {
            var path = FileManager.CombinePath(new string[]
            {
                GetPersonalFolder(),
                MediaFolder
            });
            return path;
        }
        public void ClearMediaFolder()
        {
            FileManager.ClearDirectory(GetMediaFolderPath());
        }

        public async Task<string> TakePhotoAsync()
        {
            try
            {
                if (!CrossMedia.Current.IsCameraAvailable)
                {
                    await App.Current.MainPage.DisplayAlert("カメラを使用できません", "この端末にはカメラ機能がありません", "OK");
                    return null;
                }
                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    DefaultCamera = CameraDevice.Rear,
                    CompressionQuality = 80,
                    SaveToAlbum = true,
                    SaveMetaData = false
                });
                if (file != null)
                    System.Diagnostics.Debug.WriteLine("TakePhotoAsync filePath :  " + file.Path);

                return file?.Path;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("TakePhotoAsync error :  " + ex);
                return null;
            }
        }
        public async Task<string> PickPhotoAsync()
        {
            try
            {
                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await App.Current.MainPage.DisplayAlert("ライブラリを使用できません", "この端末ではライブラリにアクセスできません", "OK");
                    return null;
                }
                var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    SaveMetaData = false,
                    CompressionQuality = 80
                });
                if (file != null)
                    System.Diagnostics.Debug.WriteLine("PickPhotoAsync filePath :  " + file.Path);

                return file?.Path;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("PickPhotoAsync error :  " + ex);
                return null;
            }
        }
        public async Task<List<string>> PickPhotoAsyncGetPathAndAlbumPath()
        {
            try
            {
                var ret = await CheckPermission(Permission.Photos);
                if (!ret)
                    return null;

                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    await App.Current.MainPage.DisplayAlert("ライブラリを使用できません", "この端末ではライブラリにアクセスできません", "OK");
                    return null;
                }
                var file = await CrossMedia.Current.PickPhotoAsync(new Plugin.Media.Abstractions.PickMediaOptions
                {
                    SaveMetaData = false,
                    CompressionQuality = 80
                });
                if (file != null)
                {
                    System.Diagnostics.Debug.WriteLine("PickPhotoAsync filePath :  " + file.Path);
                    var list = new List<string>();
                    list.Add(file.Path);
                    list.Add(file.AlbumPath);
                    return list;
                }
                return null;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("PickPhotoAsyncGetPathAndAlbumPath error :  " + ex);
                return null;
            }
        }
        public async Task<bool> CheckPermission(Permission requestPermission)
        {
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(requestPermission);
            if (status != PermissionStatus.Granted)
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(requestPermission);
                if (results.ContainsKey(requestPermission) &&
                    results[requestPermission] != PermissionStatus.Granted)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
