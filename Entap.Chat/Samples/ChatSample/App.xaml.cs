using System;
using Xamarin.Forms;

namespace ChatSample
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());

            var service = new ChatService();
            Entap.Chat.Settings.Current.Init(service);
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
