using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace ChatSample
{
    public partial class SettingPage : ContentPage
    {
        public SettingPage()
        {
            InitializeComponent();
            this.BindingContext = new SettingPageViewModel();
        }
    }
}
