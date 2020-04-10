using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace ChatSample
{
    public partial class ChatPage : ContentPage
    {
        public ChatPage(Room room)
        {
            InitializeComponent();
            this.BindingContext = new ChatPageViewModel(room);
        }
    }
}
