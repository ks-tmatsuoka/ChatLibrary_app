using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Entap.Chat.Modules;

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
                ProcessManager.Current.Invoke(nameof(DownloadButton), async () =>
                {
                    await Settings.Current.ChatService.ImageDownload(imageUrl);
                });
            };

            ShareButton.Clicked += (sender, args) =>
            {
                ProcessManager.Current.Invoke(nameof(ShareButton), async () =>
                {
                    // TODO パーミッションチェック

                    await Settings.Current.ChatService.ImageShare(imageUrl);
                });
            };
        }
    }
}
