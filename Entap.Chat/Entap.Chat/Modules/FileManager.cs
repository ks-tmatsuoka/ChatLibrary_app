using System;
using System.IO;

namespace Entap.Chat.Modules
{
    public class FileManager
    {
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

        public static bool ClearDirectory(string path)
        {
            try
            {
                if (FolderExists(path))
                {
                    var dir = new DirectoryInfo(path);
                    var files = dir.GetFiles();
                    foreach (var file in files)
                    {
                        file.Delete();
                    }
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("ClearDirectory NotExists");
                    return false;
                }
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine("ClearDirectory " + ex);
                return false;
            }
        }

        public static bool FolderExists(string folderPath)
        {
            try
            {
                var dir = new DirectoryInfo(folderPath);
                return dir.Exists;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("FolderExists " + ex);
                return false;
            }
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
    }
}
