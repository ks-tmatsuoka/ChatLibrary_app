using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ChatSample
{
    public partial class ImagePreviewPage : ContentPage
    {
        public ImagePreviewPage(string imageUrl)
        {
            InitializeComponent();
            this.BindingContext = new ImagePreviewPageViewModel(imageUrl);
        }
    }
}
