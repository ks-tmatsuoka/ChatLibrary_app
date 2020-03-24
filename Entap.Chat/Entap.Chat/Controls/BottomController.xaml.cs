using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Entap.Chat
{
    public partial class BottomController : CustomContentView
    {
        const int baseControllerHeight = 48;
        public BottomController()
        {
            InitializeComponent();

            this.SizeChanged += (sender, args) =>
            {
                var safearea = DependencyService.Get<IDisplayService>().GetSafeArea();
                this.Controller.Padding = new Thickness(0, 0, 0, safearea.Bottom);
            };
        }
    }
}
