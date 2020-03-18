using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ChatSample
{
    public class PageViewModelBase : BindableBase
    {
        public PageViewModelBase()
        {
        }

        public Command PushPageCommand => new Command(async (obj) => await PushPageAsync(GeneratePage(obj)));

        public async Task PushPageAsync(Page page)
        {
            await App.Current.MainPage.Navigation.PushAsync(page);
        }

        Page GeneratePage(object pageType)
        {
            if (pageType == null) return null;
            if (!(pageType is Type type)) return null;
            return Activator.CreateInstance(type) as Page;
        }
    }
}
