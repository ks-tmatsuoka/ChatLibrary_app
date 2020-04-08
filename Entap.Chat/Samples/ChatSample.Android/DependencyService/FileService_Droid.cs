using System;
using Android.Content;
using Android.Webkit;
using V4 = Android.Support.V4;
using Uri = Android.Net.Uri;
using Environment = Android.OS.Environment;
using Entap.Chat.Android;
using Xamarin.Forms;
using Android.OS;
using ChatSample.Android;

[assembly: Dependency(typeof(FileService_Droid))]
namespace ChatSample.Android
{
    public class FileService_Droid : IFileService
    {
        public void OpenShareMenu(string filePath, ref string err)
        {
            try
            {
                Intent intent = new Intent();

                ////MimeTypeの取得
                //string mimetype = GetMimeType(filePath);
                //if (String.IsNullOrEmpty(mimetype))
                //{
                //    //MimeTypeが取得できなかった場合
                //    //アクションにACTION_SENDを指定して暗黙的インテントを呼び出すことで、
                //    //インストールされているアプリで対応可能なものが列挙されます。
                //    mimetype = "*/*";
                //    intent.SetAction(Intent.ActionSend);
                //}
                //else
                //{
                //    //MimeTypeが取得できた場合
                //    intent.SetAction(Intent.ActionView);
                //}

                string mimetype = "*/*";
                intent.SetAction(Intent.ActionSend);

                //URLとmimetypeを取得
                Java.IO.File file = new Java.IO.File(filePath);
                Uri uri = null;
                if ((int)Build.VERSION.SdkInt < (int)(BuildVersionCodes.N))
                {
                    //Android6.0以前
                    uri = Uri.FromFile(file);
                }
                else
                {
                    //Android7.0以降
                    uri = V4.Content.FileProvider.GetUriForFile(
                        Platform.Activity.ApplicationContext,
                        Platform.Activity.PackageName + ".fileprovider",
                        file
                    );
                }

                intent.SetFlags(ActivityFlags.ClearTop);
                intent.SetFlags(ActivityFlags.NewTask);
                intent.SetAction(Intent.ActionSend);
                intent.SetType("*/*");
                intent.PutExtra(Intent.ExtraStream, uri);
                intent.AddFlags(ActivityFlags.GrantReadUriPermission);

                var chooserIntent = Intent.CreateChooser(intent, "");
                chooserIntent.SetFlags(ActivityFlags.ClearTop);
                chooserIntent.SetFlags(ActivityFlags.NewTask);
                Platform.Activity.ApplicationContext.StartActivity(chooserIntent);
            }
            catch (ActivityNotFoundException ex)
            {
                err = ex.Message;
            }
        }

        public string GetDownloadFolderPath()
        {
            var path = Environment.GetExternalStoragePublicDirectory(Environment.DirectoryDownloads).AbsolutePath;
            return path;
        }

        public string GetMediaFolderPath()
        {
            var path = Platform.Activity.GetExternalFilesDir(Environment.DirectoryPictures).AbsolutePath + "/ChatLibDir/";
            //var path = CrossCurrentActivity.Current.Activity.GetExternalFilesDir(Android.OS.Environment.DirectoryPictures).AbsolutePath + "/temp";
            using (var mediaStorageDir = new Java.IO.File(path))
            {
                if (!mediaStorageDir.Exists())
                {
                    if (!mediaStorageDir.Mkdirs())
                        throw new Java.IO.IOException("Couldn't create directory, have you added the WRITE_EXTERNAL_STORAGE permission?");

                }
            }
            return path;
        }

        public bool? SaveImageiOSLibrary(string filePath)
        {
            return false;
        }
    }
}
