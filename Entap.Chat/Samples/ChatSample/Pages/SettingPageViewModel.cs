using System;
using Xamarin.Forms;

namespace ChatSample
{
    public class SettingPageViewModel : BindableBase
    {
        public SettingPageViewModel()
        {
        }

        public Command MyInfoCmd => new Command(() =>
        {
            var page = new MyInfoPage();
            App.Current.MainPage.Navigation.PushAsync(page);
        });

        public Command MoveContactPageCmd => new Command(() =>
        {
            var page = new ContactAddressePage();
            App.Current.MainPage.Navigation.PushAsync(page);
        });

        public Command CreateRoomCmd => new Command(() =>
        {
            var page = new CreateRoomPage(1);
            App.Current.MainPage.Navigation.PushAsync(page);
        });
    }
}
