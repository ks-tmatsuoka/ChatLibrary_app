using System;
using Xamarin.Forms;

namespace ChatSample
{
    public class VideoPreviewPageViewModel : BindableBase
    {
        public VideoPreviewPageViewModel(string url)
        {
            VideoUrl = url;
            var safearea = DependencyService.Get<IDisplayService>().GetSafeArea();
            BottomSafeArea = safearea.Bottom;
            TopSafeArea = safearea.Top + 16;
        }

        public Command CloseCmd => new Command(() =>
        {
            App.Current.MainPage.Navigation.PopModalAsync();
        });

        public Command DownloadCmd => new Command(async () =>
        {
            await MediaManager.VideoDownload(VideoUrl);
        });

        public Command ShareCmd => new Command(async () =>
        {
            await MediaManager.VideoShare(VideoUrl);
        });

        private double bottomSafeArea;
        public double BottomSafeArea
        {
            get
            {
                return bottomSafeArea;
            }
            set
            {
                if (bottomSafeArea != value)
                {
                    bottomSafeArea = value;
                    OnPropertyChanged("BottomSafeArea");
                }
            }
        }

        private double topSafeArea;
        public double TopSafeArea
        {
            get
            {
                return topSafeArea;
            }
            set
            {
                if (topSafeArea != value)
                {
                    topSafeArea = value;
                    OnPropertyChanged("TopSafeArea");
                }
            }
        }

        private string videoUrl;
        public string VideoUrl
        {
            get
            {
                return videoUrl;
            }
            set
            {
                if (videoUrl != value)
                {
                    videoUrl = value;
                    OnPropertyChanged("VideoUrl");
                }
            }
        }
    }
}
