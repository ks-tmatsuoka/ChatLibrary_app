using System;
using Xamarin.Forms;

namespace ChatSample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Device.SetFlags(new string[] { "MediaElement_Experimental" });

            MainPage = new NavigationPage(new MainPage());

            Entap.Chat.Settings.Current.Init(new ChatService(), new ChatControlService());
            FileManager.CreateFolders();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
