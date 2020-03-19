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

            var messaging = new Messaging();
            Entap.Chat.Settings.Current.Init(messaging);
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
