using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ChatSample
{
    public class FileManager
    {
        

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

        public static bool DeleteDirectory(string path)
        {
            try
            {
                if (FolderExists(path))
                {
                    var dir = new DirectoryInfo(path);
                    dir.Delete(true);
                    return true;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("DeleteDirectory NotExists");
                    return false;
                }
            }
            catch (SystemException ex)
            {
                System.Diagnostics.Debug.WriteLine("DeleteDirectory " + ex);
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

        public static bool FileCopy(string sourceFileName, string destFileName)
        {
            try
            {
                File.Copy(sourceFileName, destFileName, true);
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("FileCopy " + ex);
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
                    System.Diagnostics.Debug.WriteLine("FileNotFpound ");
                    return false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("FileDelete " + ex);
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
                System.Diagnostics.Debug.WriteLine("FileExists " + ex);
                return false;
            }
        }

        public static bool MakeDir(string folderPath)
        {
            try
            {
                var dir = new DirectoryInfo(folderPath);
                dir.Create();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("MakeDir " + ex);
                return false;
            }
        }

        public static bool SaveFile(string filePath, byte[] buf)
        {
            try
            {
                var file = new FileInfo(filePath);
                using (var stream = file.OpenWrite())
                {
                    stream.Write(buf, 0, buf.Length);
                }
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("SaveFile " + ex);
                return false;
            }
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
        public static string ReadText(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath, System.Text.Encoding.ASCII);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ReadText " + ex);
                return null;
            }
        }
    }
}
