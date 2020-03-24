using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Entap.Chat
{
    public class DynamicResizedEditor : Editor
    {
        public DynamicResizedEditor()
        {   
        }

        public event EventHandler Resized = delegate { };

        public void Resize(int lineCount)
        {
            if (lineCount <= MaxDisplayLineCount)
            {
                // サイズの強制変更。
                this.InvalidateMeasure();

                // リサイズ通知
                Resized?.Invoke(this, EventArgs.Empty);
            }
        }

        #region CornerRadius BindableProperty
        public static readonly BindableProperty CornerRadiusProperty =
            BindableProperty.Create(nameof(CornerRadius), typeof(int), typeof(DynamicResizedEditor), 0,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((DynamicResizedEditor)bindable).CornerRadius = (int)newValue);

        public int CornerRadius
        {
            get { return (int)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        #endregion

        //#region UnfocusCommand BindableProperty
        //public static readonly BindableProperty UnfocusCommandProperty =
        //    BindableProperty.Create(nameof(UnfocusCommand), typeof(ICommand), typeof(DynamicResizedEditor), null,
        //        propertyChanged: (bindable, oldValue, newValue) =>
        //            ((DynamicResizedEditor)bindable).UnfocusCommand = (ICommand)newValue);

        //public ICommand UnfocusCommand
        //{
        //    get { return (ICommand)GetValue(UnfocusCommandProperty); }
        //    set { SetValue(UnfocusCommandProperty, value); }
        //}
        //#endregion

        //#region IsInputAccessoryViewHiddeniOS BindableProperty
        //public static readonly BindableProperty IsInputAccessoryViewHiddeniOSProperty =
        //    BindableProperty.Create(nameof(IsInputAccessoryViewHiddeniOS), typeof(bool), typeof(DynamicResizedEditor), false,
        //        propertyChanged: (bindable, oldValue, newValue) =>
        //            ((DynamicResizedEditor)bindable).IsInputAccessoryViewHiddeniOS = (bool)newValue);

        //public bool IsInputAccessoryViewHiddeniOS
        //{
        //    get { return (bool)GetValue(IsInputAccessoryViewHiddeniOSProperty); }
        //    set { SetValue(IsInputAccessoryViewHiddeniOSProperty, value); }
        //}
        //#endregion

        #region MaxDisplayLineCount BindableProperty
        public static readonly BindableProperty MaxDisplayLineCountProperty =
            BindableProperty.Create(nameof(Placeholder), typeof(int), typeof(DynamicResizedEditor), 4,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((DynamicResizedEditor)bindable).MaxDisplayLineCount = (int)newValue);

        public int MaxDisplayLineCount
        {
            get { return (int)GetValue(MaxDisplayLineCountProperty); }
            set { SetValue(MaxDisplayLineCountProperty, value); }
        }
        #endregion

        //#region ForcedMinimumHeight BindableProperty
        //public static readonly BindableProperty ForcedMinimumHeightProperty =
        //    BindableProperty.Create(nameof(Placeholder), typeof(bool), typeof(DynamicResizedEditor), false,
        //        propertyChanged: (bindable, oldValue, newValue) =>
        //                            ((DynamicResizedEditor)bindable).ForcedMinimumHeight = (bool)newValue);

        //public bool ForcedMinimumHeight
        //{
        //    get { return (bool)GetValue(ForcedMinimumHeightProperty); }
        //    set { SetValue(ForcedMinimumHeightProperty, value); }
        //}
        //#endregion
    }
}
