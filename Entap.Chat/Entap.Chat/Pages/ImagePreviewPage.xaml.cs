using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Entap.Chat
{
    public partial class ImagePreviewPage : ContentPage
    {
        public ImagePreviewPage(string imageUrl)
        {
            InitializeComponent();
            Menu.IsVisible = false;
            Img.Source = imageUrl;

            var gs = new TapGestureRecognizer();
            gs.Tapped += (sender, args) =>
            {
                Menu.IsVisible = !Menu.IsVisible;
            };
            Img.GestureRecognizers.Add(gs);
            Menu.GestureRecognizers.Add(gs);

            var safearea = DependencyService.Get<IDisplayService>().GetSafeArea();
            Menu.Padding = new Thickness(16, safearea.Top + 16, 16, 20 + safearea.Bottom);

            CloseButton.Clicked += (sender, args) =>
            {
                ProcessManager.Current.Invoke(nameof(this.CloseButton), async () =>
                {
                    await Application.Current.MainPage.Navigation.PopModalAsync();
                });
            };

            DownloadButton.Clicked += (sender, args) =>
            {
                ProcessManager.Current.Invoke(nameof(this.CloseButton), async () =>
                {
                    var dlFolderPath = DependencyService.Get<IFileService>().GetDownloadFolderPath();
                    var extension = System.IO.Path.GetExtension(imageUrl);
                    string filePath = dlFolderPath;
                    if (extension.ToLower() == ".jpeg" || extension.ToLower() == ".jpg")
                    {
                        filePath += "/" + Guid.NewGuid() + ".jpeg";
                    }
                    else if (extension.ToLower() == ".pdf")
                    {
                        filePath += "/" + Guid.NewGuid() + ".pdf";
                    }
                    else
                    {
                        filePath += "/" + Guid.NewGuid() + ".png";
                    }
                    bool? dlResult;
                    if (Device.RuntimePlatform == Device.Android)
                        dlResult = await ImageManager.DownloadWebImageFile(imageUrl, filePath);
                    else
                        dlResult = DependencyService.Get<IFileService>().SaveImageiOSLibrary(imageUrl);
                    if (dlResult == true)
                        await Application.Current.MainPage.DisplayAlert("", "保存しました", "閉じる");
                    else if (dlResult == false)
                        await Application.Current.MainPage.DisplayAlert("", "保存できませんでした", "閉じる");
                });
            };

            ShareButton.Clicked += (sender, args) =>
            {
                ProcessManager.Current.Invoke(nameof(this.CloseButton), async () =>
                {
                    // TODO パーミッションチェック

                    await ImageManager.ImageShare(imageUrl);
                });
            };
        }
    }
}
