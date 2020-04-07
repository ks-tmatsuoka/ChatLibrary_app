using System;
using System.IO;
using Xamarin.Forms;

namespace Entap.Chat
{
    public class FileManager
    {

        const string AppRootFolderName = "EntapChatApp";
        public enum AppDataFolders
        {
            temp,
            SendImage,
            NotSendImage
        }

        public static byte[] ReadBytes(string filePath)
        {
            try
            {
                return System.IO.File.ReadAllBytes(filePath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ReadBytes " + ex);
                return null;
            }
        }

        public static bool FileCopy(string sourceFileName, string destFileName)
        {
            try
            {
                File.Copy(sourceFileName, destFileName, true);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool CreateFolders()
        {
            // App
            var appResult = FileManager.MakeDir(
                FileManager.CombinePath(new string[] {
                FileManager.GetPersonalFolder(),
                AppRootFolderName
            }));

            if (!appResult)
                return false;

            // AppDataFolders
            foreach (var item in Enum.GetValues(typeof(AppDataFolders)))
            {
                var result = FileManager.MakeDir(GetContentsPath((AppDataFolders)item));
                if (!result)
                    return false;
            }

            return true;
        }

        public static bool MakeDir(string folderPath)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine("Create Folder: " + folderPath);
                var dir = new DirectoryInfo(folderPath);
                dir.Create();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string CombinePath(string[] paths)
        {
            try
            {
                return Path.Combine(paths);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("CombinePath " + ex);
                return null;
            }
        }

        public static string GetPersonalFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Personal);
        }

        // アプリ管理データのフルパスを取得する。
        public static string GetContentsPath(AppDataFolders folder)
        {
            var path = FileManager.CombinePath(new string[]{
                FileManager.GetPersonalFolder(),
                AppRootFolderName,
                Enum.GetName(typeof(AppDataFolders),folder)
            });
            return path;
        }

        public static bool FileExists(string filePath)
        {
            try
            {
                var file = new FileInfo(filePath);
                return file.Exists;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static bool FileDelete(string path)
        {
            try
            {
                if (FileExists(path))
                {
                    var file = new FileInfo(path);
                    file.Delete();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
