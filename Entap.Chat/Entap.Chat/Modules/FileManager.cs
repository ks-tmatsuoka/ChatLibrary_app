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
