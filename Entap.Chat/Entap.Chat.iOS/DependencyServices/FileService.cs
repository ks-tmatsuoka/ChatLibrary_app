using System;
using System.IO;
using Entap.Chat.iOS;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileService))]
namespace Entap.Chat.iOS
{
    public class FileService : IFileService
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
