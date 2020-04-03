using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;

namespace Entap.Chat
{
    public partial class BottomController : CustomContentView
    {
        public BottomController()
        {
            InitializeComponent();

            var safearea = DependencyService.Get<IDisplayService>().GetSafeArea();
            this.Controller.Padding = new Thickness(0, 0, 0, safearea.Bottom);

            this.SendButton.Clicked += (sender, e) => ProcessManager.Current.Invoke(nameof(this.SendButton), async () => await SendMessage());
            this.SendPhotoButton.Clicked += (sender, e) => ProcessManager.Current.Invoke(nameof(this.SendPhotoButton), async () => await SendPhoto());
            this.SendImgButton.Clicked += (sender, e) => ProcessManager.Current.Invoke(nameof(this.SendImgButton), async () => await SendImg());
            Settings.Current.Messaging.UpdateData(this.ChatList.Messages);

            this.Controller.BackgroundColor = BottomControllerBackgroundColor;

            if (BottomControllerIconStyle == ControllerIconStyles.Dark)
            {
                SendPhotoButton.ImageSource = "camera_icon_dark.png";
                SendImgButton.ImageSource = "library_icon_dark.png";
                SendButton.ImageSource = "send_icon_dark.png";
            }
            else
            {
                SendPhotoButton.ImageSource = "camera_icon.png";
                SendImgButton.ImageSource = "library_icon.png";
                SendButton.ImageSource = "send_icon.png";
            }
        } 

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == ChatViewBackgroundColorProperty.PropertyName)
            {
                ChatList.BackgroundColor = ChatViewBackgroundColor;
            }
            else if (propertyName == MyMessageViewBackgroundColorProperty.PropertyName)
            {
                ChatList.BackgroundColor = MyMessageViewBackgroundColor;
            }
            else if (propertyName == OtherMessageViewBackgroundColorProperty.PropertyName)
            {
                ChatList.BackgroundColor = OtherMessageViewBackgroundColor;
            }            
        }

        public double ImageMessageSize
        {
            get
            {
                var rect = DependencyService.Get<IDisplayService>().GetDisplaySize();
                return rect.Width * 0.5;
            }
        }

        async Task SendMessage(MessageBase msg=null)
        {
            if (msg is null)
            {
                if (string.IsNullOrEmpty(this.MsgEditor.Text))
                    return;
                msg = new MessageBase { MessageId = ChatList.GetNotSendMessageId(), Text = MsgEditor.Text, IsAlreadyRead = false, MessageType=1, SendUserId = UserDataManager.Instance.UserId };
                this.MsgEditor.Text = "";
            }
            else
            {
                var oldMsgIndex = ChatList.Messages.IndexOf(msg);
                ChatList.Messages.RemoveAt(oldMsgIndex);
                msg.ResendVisible = false;
            }
            ChatList.AddMessage(msg);
            

            var newMsgId = await Settings.Current.Messaging.SendMessage(msg);
            var index = ChatList.Messages.IndexOf(msg);
            if (newMsgId < 0)
            {
                // 通信エラー
                ChatList.Messages[index].ResendVisible = true;
            }
            else
            {
                // サーバへ送信できた段階でメッセージの表示位置を再確認
                if (index == ChatList.Messages.Count - 1)
                {
                    //ChatList.Messages[index].Id = newMsgId;
                    ChatList.Messages[index].MessageId = ChatList.Messages.Max(w => w.MessageId) + 1; // テストコード
                }
                else
                {
                    ChatList.Messages.RemoveAt(index);
                    //msg.Id = newMsgId;
                    msg.MessageId = ChatList.Messages.Max(w => w.MessageId) + 1; // テストコード
                    ChatList.Messages.Add(msg);
                }
            }
        }

        async Task SendPhoto()
        {
            var imgPath = await Settings.Current.Messaging.TakePicture();
            if (string.IsNullOrEmpty(imgPath))
                return;
            byte[] bytes = FileManager.ReadBytes(imgPath);
            string extension = System.IO.Path.GetExtension(imgPath);
            string name = Guid.NewGuid().ToString() + extension;
            if (bytes == null || bytes.Length < 1)
            {
                return;
            }
            var msg = new MessageBase { MessageId = ChatList.GetNotSendMessageId(), ImageUrl = imgPath, MessageType=2, SendUserId=UserDataManager.Instance.UserId };
            ChatList.AddMessage(msg);
            var newMsgId = await Settings.Current.Messaging.SendMessage(msg);
            var index = ChatList.Messages.IndexOf(msg);
            if (newMsgId < 0)
            {
                // 通信エラー
                ChatList.Messages[index].ResendVisible = true;
            }
            else
            {
                // サーバへ送信できた段階でメッセージの表示位置を再確認
                if (index == ChatList.Messages.Count - 1)
                {
                    //ChatList.Messages[index].Id = newMsgId;
                    ChatList.Messages[index].MessageId = ChatList.Messages.Max(w => w.MessageId) + 1; // テストコード
                }
                else
                {
                    ChatList.Messages.RemoveAt(index);
                    //msg.Id = newMsgId;
                    msg.MessageId = ChatList.Messages.Max(w => w.MessageId) + 1; // テストコード
                    ChatList.Messages.Add(msg);
                }
            }
        }

        async Task SendImg(MessageBase msg=null)
        {
            if (msg is null)
            {
                var imgPath = await Settings.Current.Messaging.SelectImage();
                if (string.IsNullOrEmpty(imgPath))
                    return;
                byte[] bytes = FileManager.ReadBytes(imgPath);
                string extension = System.IO.Path.GetExtension(imgPath);
                string name = Guid.NewGuid().ToString() + extension;
                if (bytes == null || bytes.Length < 1)
                {
                    return;
                }
                msg = new MessageBase { MessageId = ChatList.GetNotSendMessageId(), ImageUrl = imgPath, MessageType = 2, SendUserId = UserDataManager.Instance.UserId };
            }
            else
            {
                var oldMsgIndex = ChatList.Messages.IndexOf(msg);
                ChatList.Messages.RemoveAt(oldMsgIndex);
                msg.ResendVisible = false;
            }
            ChatList.AddMessage(msg);
            var newMsgId = await Settings.Current.Messaging.SendMessage(msg);
            var index = ChatList.Messages.IndexOf(msg);
            if (newMsgId < 0)
            {
                // 通信エラー
                ChatList.Messages[index].ResendVisible = true;
            }
            else
            {
                // サーバへ送信できた段階でメッセージの表示位置を再確認
                if (index == ChatList.Messages.Count - 1)
                {
                    //ChatList.Messages[index].Id = newMsgId;
                    ChatList.Messages[index].MessageId = ChatList.Messages.Max(w => w.MessageId) + 1; // テストコード
                }
                else
                {
                    ChatList.Messages.RemoveAt(index);
                    //msg.Id = newMsgId;
                    msg.MessageId = ChatList.Messages.Max(w => w.MessageId) + 1; // テストコード
                    ChatList.Messages.Add(msg);
                }
            }
        }

        public Command ImageTapCommand => new Command((pm) =>
        {
            ProcessManager.Current.Invoke(nameof(this.SendButton), async () =>
            {
                var imagePath = pm as string;
                await Application.Current.MainPage.Navigation.PushModalAsync(new ImagePreviewPage(imagePath));
            });
        });

        public Command ImageShareCommand => new Command((pm) =>
        {
            ProcessManager.Current.Invoke(nameof(this.SendButton), async()=>
            {
                // TODO パーミッションチェック

                var imagePath = pm as string;
                imagePath = "https://brave.entap.dev/storage/user_icon.png";
                await ImageManager.ImageShare(imagePath);
            });
        });


        public Command ResendCommand => new Command((obj) =>
        {
            ProcessManager.Current.Invoke(nameof(this.SendButton), async () =>
            {
                var msg = obj as MessageBase;
                var button = new string[]{ "再送する","取り消し" };
                var result = await Application.Current.MainPage.DisplayActionSheet(null, "キャンセル", null, button);
                if (result.Equals(button[0]))
                {
                    if (msg.MessageType == 1)
                    {
                        await SendMessage(msg);
                    }
                    else if (msg.MessageType == 2)
                    {
                        await SendImg(msg);
                    }
                }
                else if (result.Equals(button[1]))
                {
                    ChatList.Messages.Remove(msg);
                }
            });
        });
        

        #region ChatViewBackgroundColor BindableProperty
        public static readonly BindableProperty ChatViewBackgroundColorProperty =
            BindableProperty.Create(nameof(ChatViewBackgroundColor), typeof(Color), typeof(BottomController), Color.Red,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((BottomController)bindable).ChatViewBackgroundColor = (Color)newValue);
        public Color ChatViewBackgroundColor
        {
            get { return (Color)GetValue(ChatViewBackgroundColorProperty); }
            set { SetValue(ChatViewBackgroundColorProperty, value); }
        }
        #endregion

        #region MyMessageViewBackgroundColor BindableProperty
        public static readonly BindableProperty MyMessageViewBackgroundColorProperty =
            BindableProperty.Create(nameof(MyMessageViewBackgroundColor), typeof(Color), typeof(BottomController), Color.LightGray,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((BottomController)bindable).MyMessageViewBackgroundColor = (Color)newValue);
        public Color MyMessageViewBackgroundColor
        {
            get { return (Color)GetValue(MyMessageViewBackgroundColorProperty); }
            set { SetValue(MyMessageViewBackgroundColorProperty, value); }
        }
        #endregion

        #region OtherMessageViewBackgroundColor BindableProperty
        public static readonly BindableProperty OtherMessageViewBackgroundColorProperty =
            BindableProperty.Create(nameof(OtherMessageViewBackgroundColor), typeof(Color), typeof(BottomController), Color.DarkGray,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((BottomController)bindable).OtherMessageViewBackgroundColor = (Color)newValue);
        public Color OtherMessageViewBackgroundColor
        {
            get { return (Color)GetValue(OtherMessageViewBackgroundColorProperty); }
            set { SetValue(OtherMessageViewBackgroundColorProperty, value); }
        }
        #endregion

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

        public enum ControllerIconStyles
        {
            Light,
            Dark
        }
    }
}
