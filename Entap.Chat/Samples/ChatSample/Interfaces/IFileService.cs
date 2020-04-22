using System;
namespace ChatSample
{
    public interface IFileService
    {
        void OpenShareMenu(string filePath, ref string err);
        string GetMediaFolderPath();
        string GetDownloadFolderPath();
        bool? SaveImageiOSLibrary(string filePath);
        bool? SaveVideoiOSLibrary(string filePath);
    }
}
