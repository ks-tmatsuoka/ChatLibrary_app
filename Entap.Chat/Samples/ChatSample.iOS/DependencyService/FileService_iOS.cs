using System;
using System.IO;
using System.Threading.Tasks;
using AssetsLibrary;
using ChatSample.iOS;
using CoreImage;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileService_iOS))]
namespace ChatSample.iOS
{
    public class FileService_iOS : IFileService
    {
        public void OpenShareMenu(string filePath, ref string err)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            var controller = GetVisibleViewController();

            // UIActivityViewController
            //var item = UIActivity.FromObject(filePath);
            //var activityItems = new Foundation.NSObject[] { item };
            //UIActivity[] applicationActivities = null;
            //var activityController = new UIActivityViewController(activityItems, applicationActivities);
            //controller.PresentViewController(activityController, true, null);

            //QuickLook
            //FileInfo fi = new FileInfo(filePath);
            //QLPreviewItemBundle prevItem = new QLPreviewItemBundle(fi.Name, fi.FullName);
            //QLPreviewController previewController = new QLPreviewController();
            //previewController.DataSource = new PreviewControllerDS(prevItem);
            //if (controller != null)
            //{
            //    //GetVisibleViewController().PresentViewController(previewController, true, null);
            //    GetVisibleViewController().PresentViewController(previewController, true, null);
            //}
            //else
            //{
            //    UINavigationController nav = new UINavigationController(previewController);
            //    UIApplication.SharedApplication.Windows[1].RootViewController.PresentViewController(
            //        nav, true, null);
            //}

            ////PresentOpenInMenu
            //var _previewController = UIDocumentInteractionController.FromUrl(Foundation.NSUrl.FromFilename(filePath));
            //_previewController.Delegate = new MyInteractionDelegate(controller);
            //var a = _previewController.PresentOpenInMenu(controller.View.Frame, controller.View, true);

            ////PresentOptionsMenu
            var _previewController = UIDocumentInteractionController.FromUrl(Foundation.NSUrl.FromFilename(filePath));
            _previewController.Delegate = new MyInteractionDelegate(controller);
            _previewController.PresentOptionsMenu(controller.View.Frame, controller.View, true);
        }

        public string GetDownloadFolderPath()
        {
            return null;
        }

        public bool? SaveImageiOSLibrary(string imagePath)
        {
            var cmp = new TaskCompletionSource<bool?>();
            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                if (Photos.PHPhotoLibrary.AuthorizationStatus == Photos.PHAuthorizationStatus.NotDetermined ||
                    Photos.PHPhotoLibrary.AuthorizationStatus == Photos.PHAuthorizationStatus.Authorized)
                {
                    Photos.PHPhotoLibrary.SharedPhotoLibrary.PerformChanges(() =>
                    {
                        Photos.PHAssetChangeRequest.FromImage(new UIImage(imagePath));
                    }, (success, err) =>
                    {
                        if (!success)
                        {
                            System.Diagnostics.Debug.WriteLine(err?.LocalizedDescription + System.Environment.NewLine + err?.LocalizedFailureReason);
                            cmp.SetResult(false);
                        }
                        else
                        {
                            cmp.SetResult(true);
                        }
                    });
                }
            }
            else
            {
                var uiImage = new UIImage(imagePath);
                //フォトアルバムに保存する
                uiImage.SaveToPhotosAlbum(new UIImage.SaveStatus(
                    delegate (UIImage img, NSError error) {
                        var hasError = (error != null);
                        if (!hasError)
                            App.Current.MainPage.DisplayAlert("", "保存しました", "閉じる");
                        else
                            App.Current.MainPage.DisplayAlert("", "保存できませんでした", "閉じる");
                    }));
                cmp.SetResult(null);
            }
            return cmp.Task.Result;
        }

        public string GetMediaFolderPath()
        {
            var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), "temp");
            Directory.CreateDirectory(path);

            return path;
        }

        UIViewController GetVisibleViewController()
        {
            var rootController = UIApplication.SharedApplication.KeyWindow.RootViewController;
            if (rootController.PresentedViewController == null)
                return rootController;

            if (rootController.PresentedViewController is UINavigationController)
                return ((UINavigationController)rootController.PresentedViewController).VisibleViewController;

            if (rootController.PresentedViewController is UITabBarController)
                return ((UITabBarController)rootController.PresentedViewController).SelectedViewController;

            return rootController.PresentedViewController;
        }
    }

    public class MyInteractionDelegate : UIDocumentInteractionControllerDelegate
    {
        UIViewController parent;

        public MyInteractionDelegate(UIViewController controller)
        {
            parent = controller;
        }

        public override UIViewController ViewControllerForPreview(UIDocumentInteractionController controller)
        {
            return parent;
        }
    }
}
