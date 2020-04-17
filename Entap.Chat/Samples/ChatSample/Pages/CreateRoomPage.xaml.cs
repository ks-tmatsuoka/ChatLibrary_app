using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace ChatSample
{
    public partial class CreateRoomPage : ContentPage
    {
        public CreateRoomPage(int type, int roomId=0)
        {
            InitializeComponent();
            this.BindingContext = new CreateRoomPageViewModel(type, roomId);
        }
    }
}
