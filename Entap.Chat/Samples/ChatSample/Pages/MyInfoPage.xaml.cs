using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace ChatSample
{
    public partial class MyInfoPage : ContentPage
    {
        public MyInfoPage()
        {
            InitializeComponent();
            this.BindingContext = new MyInfoPageViewModel();
        }
    }
}
