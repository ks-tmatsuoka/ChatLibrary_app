using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Entap.Chat
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
                //LogManager.WriteLog(LogManager.LogLevel.Warning, "FileCopy", ex);
                return false;
            }
        }
    }
}
