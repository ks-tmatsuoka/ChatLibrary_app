using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace ChatSample
{
    public partial class VideoPreviewPage : ContentPage
    {
        public VideoPreviewPage(string url)
        {
            InitializeComponent();
            this.BindingContext = new VideoPreviewPageViewModel(url);
        }
    }
}
