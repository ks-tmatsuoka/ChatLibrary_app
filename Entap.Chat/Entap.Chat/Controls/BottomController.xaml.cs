using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Xamarin.Forms;

namespace Entap.Chat
{
    public partial class BottomController : ContentView
    {
        public BottomController()
        {
            InitializeComponent();

            if (MenuView != null)
                Menu.Children.Add(MenuView);

            Controller.BackgroundColor = BottomControllerBackgroundColor;
            if (BottomControllerIconStyle == ControllerIconStyles.Dark)
            {
                SendButton.ImageSource = "send_icon_dark.png";
            }
            else
            {
                SendButton.ImageSource = "send_icon.png";
            }
            this.SendButton.Clicked += (sender, e) => SendCommand?.Execute(null);
            this.MsgEditor.Text = EditorText;
            MsgEditor.TextChanged += EditorTextChanged;
        }

        private void EditorTextChanged(object sender, TextChangedEventArgs e)
        {
            EditorText = this.MsgEditor.Text;
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == EditorTextProperty.PropertyName)
            {
                this.MsgEditor.Text = EditorText;
            }
            else if (propertyName == BottomControllerBackgroundColorProperty.PropertyName)
            {
                Controller.BackgroundColor = BottomControllerBackgroundColor;
            }
            else if (propertyName == BottomControllerIconStyleProperty.PropertyName)
            {
                if (BottomControllerIconStyle == ControllerIconStyles.Dark)
                {
                    SendButton.ImageSource = "send_icon_dark.png";
                }
                else
                {
                    SendButton.ImageSource = "send_icon.png";
                }
            }
            else if (propertyName == MenuViewProperty.PropertyName)
            {
                Menu.Children.Clear();
                if (MenuView != null)
                    Menu.Children.Add(MenuView);
            }
        }

        /// <summary>
        /// ページ下部のコントローラーの色のスタイル
        /// </summary>
        #region BottomControllerIconStyle BindableProperty
        public static readonly BindableProperty BottomControllerIconStyleProperty =
            BindableProperty.Create(nameof(BottomControllerIconStyle), typeof(ControllerIconStyles), typeof(BottomController), ControllerIconStyles.Light,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((BottomController)bindable).BottomControllerIconStyle = (ControllerIconStyles)newValue);
        public ControllerIconStyles BottomControllerIconStyle
        {
            get { return (ControllerIconStyles)GetValue(BottomControllerIconStyleProperty); }
            set { SetValue(BottomControllerIconStyleProperty, value); }
        }
        #endregion

        /// <summary>
        /// ページ下部のコントローラーの背景色
        /// </summary>
        #region BottomControllerBackgroundColor BindableProperty
        public static readonly BindableProperty BottomControllerBackgroundColorProperty =
            BindableProperty.Create(nameof(BottomControllerBackgroundColor), typeof(Color), typeof(BottomController), Color.Black,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((BottomController)bindable).BottomControllerBackgroundColor = (Color)newValue);
        public Color BottomControllerBackgroundColor
        {
            get { return (Color)GetValue(BottomControllerBackgroundColorProperty); }
            set { SetValue(BottomControllerBackgroundColorProperty, value); }
        }
        #endregion

        /// <summary>
        /// 送信ボタン押した際のコマンド
        /// </summary>
        #region SendCommand BindableProperty
        public static readonly BindableProperty SendCommandProperty =
            BindableProperty.Create(nameof(SendCommand), typeof(ICommand), typeof(BottomController), null,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((BottomController)bindable).SendCommand = (ICommand)newValue);
        public ICommand SendCommand
        {
            get { return (ICommand)GetValue(SendCommandProperty); }
            set { SetValue(SendCommandProperty, value); }
        }
        #endregion

        /// <summary>
        /// 入力メッセージ
        /// </summary>
        #region EditorText BindableProperty
        public static readonly BindableProperty EditorTextProperty =
            BindableProperty.Create(nameof(EditorText), typeof(string), typeof(BottomController), null,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((BottomController)bindable).EditorText = (string)newValue);
        public string EditorText
        {
            get { return (string)GetValue(EditorTextProperty); }
            set { SetValue(EditorTextProperty, value); }
        }
        #endregion

        ///// <summary>
        ///// カメラボタン押した際のコマンド
        ///// </summary>
        //#region CameraCommand BindableProperty
        //public static readonly BindableProperty CameraCommandProperty =
        //    BindableProperty.Create(nameof(CameraCommand), typeof(ICommand), typeof(BottomController), null,
        //        propertyChanged: (bindable, oldValue, newValue) =>
        //                            ((BottomController)bindable).CameraCommand = (ICommand)newValue);
        //public ICommand CameraCommand
        //{
        //    get { return (ICommand)GetValue(CameraCommandProperty); }
        //    set { SetValue(CameraCommandProperty, value); }
        //}
        //#endregion

        ///// <summary>
        ///// ライブラリボタン押した際のコマンド
        ///// </summary>
        //#region LibraryCommand BindableProperty
        //public static readonly BindableProperty LibraryCommandProperty =
        //    BindableProperty.Create(nameof(LibraryCommand), typeof(ICommand), typeof(BottomController), null,
        //        propertyChanged: (bindable, oldValue, newValue) =>
        //                            ((BottomController)bindable).LibraryCommand = (ICommand)newValue);
        //public ICommand LibraryCommand
        //{
        //    get { return (ICommand)GetValue(LibraryCommandProperty); }
        //    set { SetValue(LibraryCommandProperty, value); }
        //}
        //#endregion

        /// <summary>
        /// メニュー
        /// </summary>
        #region MenuView BindableProperty
        public static readonly BindableProperty MenuViewProperty =
            BindableProperty.Create(nameof(MenuView), typeof(View), typeof(BottomController), null,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((BottomController)bindable).MenuView = (View)newValue);
        public View MenuView
        {
            get { return (View)GetValue(MenuViewProperty); }
            set { SetValue(MenuViewProperty, value); }
        }
        #endregion
    }

    public enum ControllerIconStyles
    {
        Light,
        Dark
    }
}
