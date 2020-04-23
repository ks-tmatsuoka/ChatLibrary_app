using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Entap.Chat
{
    [Preserve(AllMembers = true)]
    public partial class DefaultMenuView : BottomContorollerMenuViewBase
    {
        public DefaultMenuView()
        {
            InitializeComponent();
            SendPhotoButton.CommandParameter = (int)BottomControllerMenuType.Camera;
            SendPhotoButton.ImageSource = SendPhotoButtonImage;
            SendImgButton.CommandParameter = (int)BottomControllerMenuType.Library;
            SendImgButton.ImageSource = SendImgButtonImage;
            this.SendPhotoButton.Clicked += (sender, e) =>
            {
                MenuCommand?.Execute(((Button)sender).CommandParameter);
            };
            this.SendImgButton.Clicked += (sender, e) =>
            {
                MenuCommand?.Execute(((Button)sender).CommandParameter);
            };
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == SendPhotoButtonImageProperty.PropertyName)
            {
                SendPhotoButton.ImageSource = SendPhotoButtonImage;
            }
            else if (propertyName == SendImgButtonImageProperty.PropertyName)
            {
                SendImgButton.ImageSource = SendImgButtonImage;
            }
        }

        #region SendImgButtonImage BindableProperty
        public static readonly BindableProperty SendPhotoButtonImageProperty =
            BindableProperty.Create(nameof(SendPhotoButtonImage), typeof(string), typeof(DefaultMenuView), null,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((DefaultMenuView)bindable).SendPhotoButtonImage = (string)newValue);
        public string SendPhotoButtonImage
        {
            get { return (string)GetValue(SendPhotoButtonImageProperty); }
            set { SetValue(SendPhotoButtonImageProperty, value); }
        }
        #endregion


        #region SendImgButtonImage BindableProperty
        public static readonly BindableProperty SendImgButtonImageProperty =
            BindableProperty.Create(nameof(SendImgButtonImage), typeof(string), typeof(DefaultMenuView), null,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((DefaultMenuView)bindable).SendImgButtonImage = (string)newValue);
        public string SendImgButtonImage
        {
            get { return (string)GetValue(SendImgButtonImageProperty); }
            set { SetValue(SendImgButtonImageProperty, value); }
        }
        #endregion
    }
}
