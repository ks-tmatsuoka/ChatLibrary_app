using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Entap.Chat
{
    public partial class BottomController : CustomContentView
    {
        const int baseControllerHeight = 48;
        public BottomController()
        {
            InitializeComponent();

            //this.SizeChanged += (sender, args) =>
            //{

            //};

            var safearea = DependencyService.Get<IDisplayService>().GetSafeArea();
            this.Controller.Padding = new Thickness(0, 0, 0, safearea.Bottom);

            //this.SendButton.Clicked += (sender, args) =>
            //{

            //};

            //this.SendButton.Clicked += (sender, e) => ProcessManager.Current.Invoke(nameof(this.SendButton), async () =>
            //{
            //    await DelayAsync();
            //});
            this.SendButton.Clicked += (sender, e) => ProcessManager.Current.Invoke(nameof(this.SendButton), async () => await SendMessage());
            this.SendPhotoButton.Clicked += (sender, e) => ProcessManager.Current.Invoke(nameof(this.SendPhotoButton), async () => await SendPhoto());
            this.SendImgButton.Clicked += (sender, e) => ProcessManager.Current.Invoke(nameof(this.SendImgButton), async () => await SendImg());

            this.Controller.BackgroundColor = BottomControllerBackgroundColor;

            if (BottomControllerIconStyle == ControllerIconStyles.Dark)
            {
                SendPhotoButton.ImageSource = "camera_icon_dark.png";
                SendImgButton.ImageSource = "library_icon_dark.png";
                SendButton.ImageSource = "send_icon_dark.png";
            }
            else
            {
                SendPhotoButton.ImageSource = "camera_icon.png";
                SendImgButton.ImageSource = "library_icon.png";
                SendButton.ImageSource = "send_icon.png";
            }
        } 

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == ChatViewBackgroundColorProperty.PropertyName)
            {
                ChatList.BackgroundColor = ChatViewBackgroundColor;
            }
            else if (propertyName == MyMessageViewBackgroundColorProperty.PropertyName)
            {
                ChatList.BackgroundColor = MyMessageViewBackgroundColor;
            }
            else if (propertyName == OtherMessageViewBackgroundColorProperty.PropertyName)
            {
                ChatList.BackgroundColor = OtherMessageViewBackgroundColor;
            }            
        }

        public double ImageMessageSize
        {
            get
            {
                var rect = DependencyService.Get<IDisplayService>().GetDisplaySize();
                return rect.Width * 0.5;
            }
        }

        async Task SendMessage()
        {
            if (string.IsNullOrEmpty(this.MsgEditor.Text))
                return;
            ChatList.AddMessage(new MyTextMessage { Id = 200, Text = MsgEditor.Text, IsAlreadyRead=true });
            this.MsgEditor.Text = "";

            var result = await Settings.Current.Messaging.SendTextMessage(this.MsgEditor.Text);
        }

        async Task SendPhoto()
        {
            var imgPath = await Settings.Current.Messaging.TakePicture();
            if (string.IsNullOrEmpty(imgPath))
                return;
            byte[] bytes = FileManager.ReadBytes(imgPath);
            string extension = System.IO.Path.GetExtension(imgPath);
            string name = Guid.NewGuid().ToString() + extension;
            if (bytes == null || bytes.Length < 1)
            {
                return;
            }
            ChatList.AddMessage(new MyImageMessage { Id = 200, ImageUrl = imgPath });
            var result = await Settings.Current.Messaging.SendImage(bytes);
        }

        async Task SendImg()
        {
            var imgPath = await Settings.Current.Messaging.SelectImage();
            if (string.IsNullOrEmpty(imgPath))
                return;
            byte[] bytes = FileManager.ReadBytes(imgPath);
            string extension = System.IO.Path.GetExtension(imgPath);
            string name = Guid.NewGuid().ToString() + extension;
            if (bytes == null || bytes.Length < 1)
            {
                return;
            }
            ChatList.AddMessage(new MyImageMessage { Id = 200, ImageUrl = imgPath });
            var result = await Settings.Current.Messaging.SendImage(bytes);
        }

        #region ChatViewBackgroundColor BindableProperty
        public static readonly BindableProperty ChatViewBackgroundColorProperty =
            BindableProperty.Create(nameof(ChatViewBackgroundColor), typeof(Color), typeof(BottomController), Color.Red,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((BottomController)bindable).ChatViewBackgroundColor = (Color)newValue);
        public Color ChatViewBackgroundColor
        {
            get { return (Color)GetValue(ChatViewBackgroundColorProperty); }
            set { SetValue(ChatViewBackgroundColorProperty, value); }
        }
        #endregion

        #region MyMessageViewBackgroundColor BindableProperty
        public static readonly BindableProperty MyMessageViewBackgroundColorProperty =
            BindableProperty.Create(nameof(MyMessageViewBackgroundColor), typeof(Color), typeof(BottomController), Color.LightGray,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((BottomController)bindable).MyMessageViewBackgroundColor = (Color)newValue);
        public Color MyMessageViewBackgroundColor
        {
            get { return (Color)GetValue(MyMessageViewBackgroundColorProperty); }
            set { SetValue(MyMessageViewBackgroundColorProperty, value); }
        }
        #endregion

        #region OtherMessageViewBackgroundColor BindableProperty
        public static readonly BindableProperty OtherMessageViewBackgroundColorProperty =
            BindableProperty.Create(nameof(OtherMessageViewBackgroundColor), typeof(Color), typeof(BottomController), Color.DarkGray,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((BottomController)bindable).OtherMessageViewBackgroundColor = (Color)newValue);
        public Color OtherMessageViewBackgroundColor
        {
            get { return (Color)GetValue(OtherMessageViewBackgroundColorProperty); }
            set { SetValue(OtherMessageViewBackgroundColorProperty, value); }
        }
        #endregion

        #region BottomControllerBackgroundColor BindableProperty
        public static readonly BindableProperty BottomControllerBackgroundColorProperty =
            BindableProperty.Create(nameof(BottomControllerBackgroundColor), typeof(Color), typeof(BottomController), Color.Black,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((BottomController)bindable).BottomControllerBackgroundColor = (Color)newValue);
        public Color BottomControllerBackgroundColor
        {
            get { return (Color)GetValue(BottomControllerBackgroundColorProperty); }
            set { SetValue(BottomControllerBackgroundColorProperty, value); }
        }
        #endregion

        #region BottomControllerIconStyle BindableProperty
        public static readonly BindableProperty BottomControllerIconStyleProperty =
            BindableProperty.Create(nameof(BottomControllerIconStyle), typeof(ControllerIconStyles), typeof(BottomController), ControllerIconStyles.Light,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((BottomController)bindable).BottomControllerIconStyle = (ControllerIconStyles)newValue);
        public ControllerIconStyles BottomControllerIconStyle
        {
            get { return (ControllerIconStyles)GetValue(BottomControllerIconStyleProperty); }
            set { SetValue(BottomControllerIconStyleProperty, value); }
        }
        #endregion

        public enum ControllerIconStyles
        {
            Light,
            Dark
        }
    }
}
