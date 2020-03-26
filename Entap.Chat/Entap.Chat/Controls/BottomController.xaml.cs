using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        }

        async Task SendMessage()
        {
            if (string.IsNullOrEmpty(this.MsgEditor.Text))
                return;
            ChatList.AddMessage(new TextMessage { Id = 200, Text = MsgEditor.Text });
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
            ChatList.AddMessage(new ImageMessage { Id = 200, ImageUrl = imgPath });
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
            ChatList.AddMessage(new ImageMessage { Id = 200, ImageUrl = imgPath });
            var result = await Settings.Current.Messaging.SendImage(bytes);
        }
    }
}
