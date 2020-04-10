using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace ChatSample
{
    public partial class ContactAddressePage : ContentPage
    {
        public ContactAddressePage()
        {
            InitializeComponent();
            this.BindingContext = new ContactAddressePageViewModel();
        }
    }
}
