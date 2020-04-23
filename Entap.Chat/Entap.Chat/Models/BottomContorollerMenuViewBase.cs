using System;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Entap.Chat
{
    [Preserve(AllMembers = true)]
    public class BottomContorollerMenuViewBase : ContentView
    {
        public BottomContorollerMenuViewBase()
        {
        }

        #region MenuCommand BindableProperty
        public static readonly BindableProperty MenuCommandProperty =
            BindableProperty.Create(nameof(MenuCommand), typeof(ICommand), typeof(BottomContorollerMenuViewBase), null,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((BottomContorollerMenuViewBase)bindable).MenuCommand = (ICommand)newValue);
        public ICommand MenuCommand
        {
            get { return (ICommand)GetValue(MenuCommandProperty); }
            set { SetValue(MenuCommandProperty, value); }
        }
        #endregion
    }
}
