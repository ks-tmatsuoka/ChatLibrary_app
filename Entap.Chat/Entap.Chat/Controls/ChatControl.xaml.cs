using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;

namespace Entap.Chat
{
    public partial class ChatControl : CustomContentView
    {
        public ChatControl()
        {
            InitializeComponent();

            var safearea = DependencyService.Get<IDisplayService>().GetSafeArea();
            this.Controller.Padding = new Thickness(0, 0, 0, safearea.Bottom);

            Controller.SendCommand = SendCommand;
            Controller.CameraCommand = CameraCommand;
            Controller.LibraryCommand = LibraryCommand;
            //this.SendImgButton.Clicked += (sender, e) => ProcessManager.Current.Invoke(nameof(this.SendImgButton), async () => await SendImg());

            Controller.BottomControllerBackgroundColor = BottomControllerBackgroundColor;
            Controller.BottomControllerIconStyle = BottomControllerIconStyle;
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
            else if (propertyName == UserIdProperty.PropertyName)
            {
                ChatList.UserId = UserId;
            }
            else if (propertyName == LastReadMessageIdProperty.PropertyName)
            {
                ChatList.LastReadMessageId = LastReadMessageId;
            }
            else if (propertyName == RoomIdProperty.PropertyName)
            {
                ChatList.RoomId = RoomId;
            }
            else if (propertyName == BottomControllerIconStyleProperty.PropertyName)
            {
                Controller.BottomControllerIconStyle = BottomControllerIconStyle;
            }
            else if (propertyName == BottomControllerBackgroundColorProperty.PropertyName)
            {
                Controller.BottomControllerBackgroundColor = BottomControllerBackgroundColor;
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

        public Command SendCommand => new Command((obj) =>
        {
            var msg = obj as MessageBase;
            ProcessManager.Current.Invoke(nameof(SendCommand), async () => await SendMessage(msg));
        });
        async Task SendMessage(MessageBase msg = null)
        {
            if (msg is null)
            {
                if (string.IsNullOrEmpty(Controller.EditorText))
                    return;
                msg = new MessageBase { MessageId = ChatList.GetNotSendMessageId(), Text = Controller.EditorText, IsAlreadyRead = false, MessageType = 1, SendUserId = UserDataManager.Instance.UserId };
                Controller.EditorText = "";
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
                ChatList.NotSendMessageSaveInStorage(ChatList.Messages[index]);
            }
            else
            {
                msg.ResendVisible = false;
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

                ChatList.NotSendMessageDeleteFromStorage(msg.NotSendId);
            }
        }

        public Command CameraCommand => new Command((obj) =>
        {
            ProcessManager.Current.Invoke(nameof(SendCommand), async () => await SendPhoto());
        });
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
            var copyImgPath = FileManager.GetContentsPath(FileManager.AppDataFolders.SendImage) + "/" + Guid.NewGuid() + extension;
            if (!FileManager.FileCopy(imgPath, copyImgPath))
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage.DisplayAlert("", "画像の取得に失敗しました", "閉じる");
                });
                return;
            }
            var msg = new MessageBase { MessageId = ChatList.GetNotSendMessageId(), ImageUrl = copyImgPath, MessageType=2, SendUserId=UserDataManager.Instance.UserId };
            await ChatAddImg(msg);
        }

        public Command LibraryCommand => new Command((obj) =>
        {
            var msg = obj as MessageBase;
            ProcessManager.Current.Invoke(nameof(SendCommand), async () => await SendImg(msg));
        });
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
                var copyImgPath = FileManager.GetContentsPath(FileManager.AppDataFolders.SendImage) + "/" + Guid.NewGuid() + extension;
                if (!FileManager.FileCopy(imgPath, copyImgPath))
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        Application.Current.MainPage.DisplayAlert("", "画像の取得に失敗しました", "閉じる");
                    });
                    return;
                }
                msg = new MessageBase { MessageId = ChatList.GetNotSendMessageId(), ImageUrl = copyImgPath, MessageType = 2, SendUserId = UserDataManager.Instance.UserId };
            }
            else
            {
                var oldMsgIndex = ChatList.Messages.IndexOf(msg);
                ChatList.Messages.RemoveAt(oldMsgIndex);
                msg.ResendVisible = false;
            }
            await ChatAddImg(msg);
        }

        async Task ChatAddImg(MessageBase msg)
        {
            ChatList.AddMessage(msg);
            var newMsgId = await Settings.Current.Messaging.SendMessage(msg);
            var index = ChatList.Messages.IndexOf(msg);
            if (newMsgId < 0)
            {
                // 通信エラー
                var delImgPath = msg.ImageUrl;
                string extension = System.IO.Path.GetExtension(delImgPath);
                ChatList.Messages[index].ResendVisible = true;
                var sendErrorImgPath = FileManager.GetContentsPath(FileManager.AppDataFolders.NotSendImage) + "/" + Guid.NewGuid() + extension;
                FileManager.FileCopy(delImgPath, sendErrorImgPath);
                ChatList.Messages[index].ImageUrl = sendErrorImgPath;
                FileManager.FileDelete(delImgPath);
                ChatList.NotSendMessageSaveInStorage(ChatList.Messages[index]);
            }
            else
            {
                msg.ResendVisible = false;
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

                ChatList.NotSendMessageDeleteFromStorage(msg.NotSendId);
            }
        }

        public Command ImageTapCommand => new Command((pm) =>
        {
            ProcessManager.Current.Invoke(nameof(ImageTapCommand), async () =>
            {
                var imagePath = pm as string;
                await Application.Current.MainPage.Navigation.PushModalAsync(new ImagePreviewPage(imagePath));
            });
        });

        public Command ImageShareCommand => new Command((pm) =>
        {
            ProcessManager.Current.Invoke(nameof(ImageShareCommand), async () =>
            {
                // TODO パーミッションチェック

                var imagePath = pm as string;
                imagePath = "https://brave.entap.dev/storage/user_icon.png";
                await ImageManager.ImageShare(imagePath);
            });
        });


        public Command ResendCommand => new Command((obj) =>
        {
            ProcessManager.Current.Invoke(nameof(ResendCommand), async () =>
            {
                var msg = obj as MessageBase;
                var button = new string[] { "再送する", "取り消し" };
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
                    if (ChatList.NotSendMessageDeleteFromStorage(msg.NotSendId))
                        ChatList.Messages.Remove(msg);
                }
            });
        });

        /// <summary>
        /// 日付のViewの背景色
        /// </summary>
        #region DateBackgroundColor BindableProperty
        public static readonly BindableProperty DateBackgroundColorProperty =
            BindableProperty.Create(nameof(DateBackgroundColor), typeof(Color), typeof(ChatControl), Color.Black,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((ChatControl)bindable).DateBackgroundColor = (Color)newValue);
        public Color DateBackgroundColor
        {
            get { return (Color)GetValue(DateBackgroundColorProperty); }
            set { SetValue(DateBackgroundColorProperty, value); }
        }
        #endregion

        /// <summary>
        /// 日付のテキストの色
        /// </summary>
        #region DateTextColor BindableProperty
        public static readonly BindableProperty DateTextColorProperty =
            BindableProperty.Create(nameof(DateTextColor), typeof(Color), typeof(ChatControl), Color.White,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((ChatControl)bindable).DateTextColor = (Color)newValue);
        public Color DateTextColor
        {
            get { return (Color)GetValue(DateTextColorProperty); }
            set { SetValue(DateTextColorProperty, value); }
        }
        #endregion

        /// <summary>
        /// チャットのViewの背景色
        /// </summary>
        #region ChatViewBackgroundColor BindableProperty
        public static readonly BindableProperty ChatViewBackgroundColorProperty =
            BindableProperty.Create(nameof(ChatViewBackgroundColor), typeof(Color), typeof(ChatControl), Color.White,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((ChatControl)bindable).ChatViewBackgroundColor = (Color)newValue);
        public Color ChatViewBackgroundColor
        {
            get { return (Color)GetValue(ChatViewBackgroundColorProperty); }
            set { SetValue(ChatViewBackgroundColorProperty, value); }
        }
        #endregion

        /// <summary>
        /// 自分のメッセージの背景色
        /// </summary>
        #region MyMessageViewBackgroundColor BindableProperty
        public static readonly BindableProperty MyMessageViewBackgroundColorProperty =
            BindableProperty.Create(nameof(MyMessageViewBackgroundColor), typeof(Color), typeof(ChatControl), Color.LightGray,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((ChatControl)bindable).MyMessageViewBackgroundColor = (Color)newValue);
        public Color MyMessageViewBackgroundColor
        {
            get { return (Color)GetValue(MyMessageViewBackgroundColorProperty); }
            set { SetValue(MyMessageViewBackgroundColorProperty, value); }
        }
        #endregion

        /// <summary>
        /// 他人のメッセージの背景色
        /// </summary>
        #region OtherMessageViewBackgroundColor BindableProperty
        public static readonly BindableProperty OtherMessageViewBackgroundColorProperty =
            BindableProperty.Create(nameof(OtherMessageViewBackgroundColor), typeof(Color), typeof(ChatControl), Color.DarkGray,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((ChatControl)bindable).OtherMessageViewBackgroundColor = (Color)newValue);
        public Color OtherMessageViewBackgroundColor
        {
            get { return (Color)GetValue(OtherMessageViewBackgroundColorProperty); }
            set { SetValue(OtherMessageViewBackgroundColorProperty, value); }
        }
        #endregion

        /// <summary>
        /// ページ下部のコントローラーの背景色
        /// </summary>
        #region BottomControllerBackgroundColor BindableProperty
        public static readonly BindableProperty BottomControllerBackgroundColorProperty =
            BindableProperty.Create(nameof(BottomControllerBackgroundColor), typeof(Color), typeof(ChatControl), Color.Black,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((ChatControl)bindable).BottomControllerBackgroundColor = (Color)newValue);
        public Color BottomControllerBackgroundColor
        {
            get { return (Color)GetValue(BottomControllerBackgroundColorProperty); }
            set { SetValue(BottomControllerBackgroundColorProperty, value); }
        }
        #endregion

        /// <summary>
        /// ページ下部のコントローラーの色のスタイル
        /// </summary>
        #region BottomControllerIconStyle BindableProperty
        public static readonly BindableProperty BottomControllerIconStyleProperty =
            BindableProperty.Create(nameof(BottomControllerIconStyle), typeof(ControllerIconStyles), typeof(ChatControl), ControllerIconStyles.Light,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((ChatControl)bindable).BottomControllerIconStyle = (ControllerIconStyles)newValue);
        public ControllerIconStyles BottomControllerIconStyle
        {
            get { return (ControllerIconStyles)GetValue(BottomControllerIconStyleProperty); }
            set { SetValue(BottomControllerIconStyleProperty, value); }
        }
        #endregion

        /// <summary>
        /// ユーザーID
        /// </summary>
        public static readonly BindableProperty UserIdProperty =
            BindableProperty.Create(nameof(UserId), typeof(int), typeof(ChatControl), -1,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((ChatControl)bindable).UserId = (int)newValue);
        public int UserId
        {
            get { return (int)GetValue(UserIdProperty); }
            set { SetValue(UserIdProperty, value); }
        }

        /// <summary>
        /// チャットのルームID
        /// </summary>
        public static readonly BindableProperty RoomIdProperty =
            BindableProperty.Create(nameof(RoomId), typeof(int), typeof(ChatControl), -1,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((ChatControl)bindable).RoomId = (int)newValue);
        public int RoomId
        {
            get { return (int)GetValue(RoomIdProperty); }
            set { SetValue(RoomIdProperty, value); }
        }

        /// <summary>
        /// 最後に既読にしたメッセージID
        /// </summary>
        public static readonly BindableProperty LastReadMessageIdProperty =
            BindableProperty.Create(nameof(LastReadMessageId), typeof(int), typeof(ChatControl), -1,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((ChatControl)bindable).LastReadMessageId = (int)newValue);
        public int LastReadMessageId
        {
            get { return (int)GetValue(LastReadMessageIdProperty); }
            set { SetValue(LastReadMessageIdProperty, value); }
        }  
    }
}
