using System;
using Entap.Chat;
using Xamarin.Forms;
namespace ChatSample
{
    public class ImagePreviewPageViewModel : BindableBase
    {
        public ImagePreviewPageViewModel(string imageUrl)
        {
            ImageUrl = imageUrl;
            var safearea = DependencyService.Get<IDisplayService>().GetSafeArea();
            MenuPadding = new Thickness(16, safearea.Top + 16, 16, 20 + safearea.Bottom);
        }

        public Command CloseCmd => new Command(() =>
        {
            App.Current.MainPage.Navigation.PopModalAsync();
        });

        public Command DownloadCmd => new Command(async() =>
        {
            await MediaManager.ImageDownload(ImageUrl);
        });

        public Command ShareCmd => new Command(async () =>
        {
            await MediaManager.ImageShare(ImageUrl);
        });

        public Command TapCommadn => new Command(() =>
        {
            MenuVisible = !MenuVisible;
        });

        private Thickness menuPadding;
        public Thickness MenuPadding
        {
            get
            {
                return menuPadding;
            }
            set
            {
                if (menuPadding != value)
                {
                    menuPadding = value;
                    OnPropertyChanged("MenuPadding");
                }
            }
        }

        private string imageUrl;
        public string ImageUrl
        {
            get
            {
                return imageUrl;
            }
            set
            {
                if (imageUrl != value)
                {
                    imageUrl = value;
                    OnPropertyChanged("ImageUrl");
                }
            }
        }

        private bool menuVisible;
        public bool MenuVisible
        {
            get
            {
                return menuVisible;
            }
            set
            {
                if (menuVisible != value)
                {
                    menuVisible = value;
                    OnPropertyChanged("MenuVisible");
                }
            }
        }
    }
}
