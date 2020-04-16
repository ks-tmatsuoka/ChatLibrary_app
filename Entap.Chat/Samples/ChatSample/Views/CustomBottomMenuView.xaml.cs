using System;
using System.Collections.Generic;
using Entap.Chat;
using Xamarin.Forms;

namespace ChatSample.Views
{
    public partial class CustomBottomMenuView : BottomContorollerMenuViewBase
    {
        public CustomBottomMenuView()
        {
            InitializeComponent();
            SendPhotoButton.CommandParameter = (int)BottomControllerMenuType.Camera;
            this.SendPhotoButton.Clicked += (sender, e) =>
            {
                MenuCommand?.Execute(((Button)sender).CommandParameter);
            };
        }
    }
}
