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
        }

        async Task SendMessage()
        {
            if (string.IsNullOrEmpty(this.MsgEditor.Text))
                return;
            ChatList.AddTextMessage(new TextMessage { Id = 0, Text = MsgEditor.Text });
            this.MsgEditor.Text = "";

            var result = await Settings.Current.Messaging.SendTextMessage(this.MsgEditor.Text);
        }
    }
}
