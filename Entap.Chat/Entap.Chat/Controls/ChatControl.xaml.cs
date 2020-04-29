using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using Entap.Chat.Modules;
using Xamarin.Forms.Internals;
using System.Threading;

namespace Entap.Chat
{
    [Preserve(AllMembers = true)]
    public partial class ChatControl : CustomContentView
    {
        public ChatControl()
        {
            InitializeComponent();

            var safearea = DependencyService.Get<IDisplayService>().GetSafeArea();
            this.Controller.Padding = new Thickness(0, 0, 0, safearea.Bottom);

            Controller.SendCommand = SendCommand;
            Controller.BottomControllerBackgroundColor = BottomControllerBackgroundColor;
            Controller.BottomControllerIconStyle = BottomControllerIconStyle;
            if (BottomControllerMenuView is null)
            {
                var defaultView = new DefaultMenuView();
                if (BottomControllerIconStyle == ControllerIconStyles.Dark)
                {
                    defaultView.SendPhotoButtonImage = "camera_icon_dark.png";
                    defaultView.SendImgButtonImage = "library_icon_dark.png";
                }
                else
                {
                    defaultView.SendPhotoButtonImage = "camera_icon.png";
                    defaultView.SendImgButtonImage = "library_icon.png";
                }
                Controller.MenuView = defaultView;
            }
            else
            {
                Controller.MenuView = BottomControllerMenuView;
            }
            ((BottomContorollerMenuViewBase)Controller.MenuView).MenuCommand = MenuCommand;
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
            else if (propertyName == LastReadMessageIdProperty.PropertyName)
            {
                ChatList.LastReadMessageId = LastReadMessageId;
            }
            else if (propertyName == RoomIdProperty.PropertyName)
            {
                ChatList.RoomId = RoomId;
            }
            else if (propertyName == RoomTypeProperty.PropertyName)
            {
                ChatList.RoomType = RoomType;
            }
            else if (propertyName == BottomControllerIconStyleProperty.PropertyName)
            {
                Controller.BottomControllerIconStyle = BottomControllerIconStyle;
                if (BottomControllerMenuView is null)
                {
                    if (BottomControllerIconStyle == ControllerIconStyles.Dark)
                    {
                        ((DefaultMenuView)Controller.MenuView).SendPhotoButtonImage = "camera_icon_dark.png";
                        ((DefaultMenuView)Controller.MenuView).SendImgButtonImage = "library_icon_dark.png";
                    }
                    else
                    {
                        ((DefaultMenuView)Controller.MenuView).SendPhotoButtonImage = "camera_icon.png";
                        ((DefaultMenuView)Controller.MenuView).SendImgButtonImage = "library_icon.png";
                    }
                }
            }
            else if (propertyName == BottomControllerBackgroundColorProperty.PropertyName)
            {
                Controller.BottomControllerBackgroundColor = BottomControllerBackgroundColor;
            }
            else if (propertyName == BottomControllerMenuViewProperty.PropertyName)
            {
                if (BottomControllerMenuView is null)
                {
                    var defaultView = new DefaultMenuView();
                    if (BottomControllerIconStyle == ControllerIconStyles.Dark)
                    {
                        defaultView.SendPhotoButtonImage = "camera_icon_dark.png";
                        defaultView.SendImgButtonImage = "library_icon_dark.png";
                    }
                    else
                    {
                        defaultView.SendPhotoButtonImage = "camera_icon.png";
                        defaultView.SendImgButtonImage = "library_icon.png";
                    }
                    Controller.MenuView = defaultView;
                }
                else
                {
                    Controller.MenuView = BottomControllerMenuView;
                }
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

        public double UserIconSize
        {
            get
            {
                return 30;
            }
        }

        public double ResendIconSize
        {
            get
            {
                return 25;
            }
        }

        public double ShareIconSize
        {
            get
            {
                return 30;
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
                msg = new TextMessage { MessageId = ChatListView.NotSendMessageId, Text = Controller.EditorText, AlreadyReadCount = 0, MessageType = (int)MessageType.Text, SendUserId = Settings.Current.ChatService.GetUserId() };
                Controller.EditorText = "";
            }
            else
            {
                var oldMsgIndex = ChatList.Messages.IndexOf(msg);
                ChatList.Messages.RemoveAt(oldMsgIndex);
                msg.ResendVisible = false;
            }
            ChatList.AddMessage(msg);
            var sendMessageResponseBase = await Settings.Current.ChatControlService.SendMessage(RoomId, msg, ChatListView.NotSendMessageId);
            var index = ChatList.Messages.IndexOf(msg);
            if (sendMessageResponseBase.MessageId < 0)
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
                    ChatList.Messages[index].MessageId = sendMessageResponseBase.MessageId;
                    ChatList.Messages[index].SendDateTime = sendMessageResponseBase.SendDateTime;
                }
                else
                {
                    ChatList.Messages.RemoveAt(index);
                    msg.MessageId = sendMessageResponseBase.MessageId;
                    msg.SendDateTime = sendMessageResponseBase.SendDateTime;
                    ChatList.Messages.Add(msg);
                }
                ChatList.SendAlreadyRead(msg);
                ChatList.NotSendMessageDeleteFromStorage(msg.NotSendId);
            }
        }

        public Command MenuCommand => new Command((obj) =>
        {
            ProcessManager.Current.Invoke(nameof(MenuCommand), async () =>
            {
                var pm = int.Parse(obj.ToString());
                var msgBases = await Settings.Current.ChatControlService.BottomControllerMenuExecute(ChatListView.NotSendMessageId, pm, RoomId, ChatList);
                if (msgBases is null)
                    return;
                if (pm == (int)BottomControllerMenuType.Camera || pm == (int)BottomControllerMenuType.Library)
                {
                    foreach (var msgBase in msgBases)
                    {
                        if (msgBase.MessageType == (int)MessageType.Image || msgBase.MessageType == (int)MessageType.Video)
                            await ChatAddMedia(msgBase);
                    }
                }
            });
        });

        async Task ResendMedia(MessageBase msg)
        {
            var oldMsgIndex = ChatList.Messages.IndexOf(msg);
            ChatList.Messages.RemoveAt(oldMsgIndex);
            msg.ResendVisible = false;
            await ChatAddMedia(msg);
        }

        CancellationTokenSource cancellationToken;
        async Task ChatAddMedia(MessageBase msg)
        {
            ChatList.AddMessage(msg);
            cancellationToken = new CancellationTokenSource();
            var sendMessageResponseBase = await Settings.Current.ChatControlService.SendMessage(RoomId, msg, ChatListView.NotSendMessageId, cancellationToken);
            var index = ChatList.Messages.IndexOf(msg);
            if (sendMessageResponseBase.MessageId < 0)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    msg.UploadProgress = 1;
                    ChatList.Messages.RemoveAt(index);
                    cancellationToken = null;
                    return;
                }
                // 通信エラー
                var delMediaPath = msg.MediaUrl;
                string extension = System.IO.Path.GetExtension(delMediaPath);
                ChatList.Messages[index].ResendVisible = true;
                var fileName = Guid.NewGuid() + extension;
                var sendErrorImgPath = Settings.Current.ChatService.GetNotSendMediaSaveFolderPath() + fileName;
                FileManager.FileCopy(delMediaPath, sendErrorImgPath);
                ChatList.Messages[index].MediaUrl = sendErrorImgPath;
                FileManager.FileDelete(delMediaPath);
                ChatList.NotSendMessageSaveInStorage(ChatList.Messages[index], fileName);
            }
            else
            {
                msg.ResendVisible = false;
                // サーバへ送信できた段階でメッセージの表示位置を再確認
                if (index == ChatList.Messages.Count - 1)
                {
                    ChatList.Messages[index].MessageId = sendMessageResponseBase.MessageId;
                    ChatList.Messages[index].SendDateTime = sendMessageResponseBase.SendDateTime;
                    ChatList.Messages[index].MediaThumbnailUrl = sendMessageResponseBase.MediaThumbnailUrl;
                }
                else
                {
                    ChatList.Messages.RemoveAt(index);
                    msg.MessageId = sendMessageResponseBase.MessageId;
                    msg.SendDateTime = sendMessageResponseBase.SendDateTime;
                    msg.MediaThumbnailUrl = sendMessageResponseBase.MediaThumbnailUrl;
                    ChatList.Messages.Add(msg);
                }
                ChatList.SendAlreadyRead(msg);
                ChatList.NotSendMessageDeleteFromStorage(msg.NotSendId);
            }
        }

        public Command MsgSendCancelCommand => new Command(() =>
        {
            if (cancellationToken != null)
                cancellationToken.Cancel();
        });

        public Command ImageTapCommand => new Command((pm) =>
        {
            ProcessManager.Current.Invoke(nameof(ImageTapCommand), async () =>
            {
                var imagePath = pm as string;
                Settings.Current.ChatControlService.MoveImagePreviewPage(imagePath);
            });
        });

        public Command VideoShareCommand => new Command((pm) =>
        {
            ProcessManager.Current.Invoke(nameof(VideoShareCommand), async () =>
            {
                var path = pm as string;
                await Settings.Current.ChatControlService.VideoShare(path);
            });
        });

        public Command VideoTapCommand => new Command((pm) =>
        {
            ProcessManager.Current.Invoke(nameof(VideoTapCommand), async () =>
            {
                var videoPath = pm as string;
                Settings.Current.ChatControlService.MoveVideoPreviewPage(videoPath);
            });
        });

        public Command ImageShareCommand => new Command((pm) =>
        {
            ProcessManager.Current.Invoke(nameof(ImageShareCommand), async () =>
            {
                var imagePath = pm as string;
                await Settings.Current.ChatControlService.ImageShare(imagePath);
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
                    if (msg.MessageType == (int)MessageType.Text)
                    {
                        await SendMessage(msg);
                    }
                    else if (msg.MessageType == (int)MessageType.Image || msg.MessageType == (int)MessageType.Video)
                    {
                        await ResendMedia(msg);
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
        /// チャットのルームタイプ
        /// </summary>
        public static readonly BindableProperty RoomTypeProperty =
            BindableProperty.Create(nameof(RoomType), typeof(int), typeof(ChatControl), 0,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((ChatControl)bindable).RoomType = (int)newValue);
        public int RoomType
        {
            get { return (int)GetValue(RoomTypeProperty); }
            set { SetValue(RoomTypeProperty, value); }
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

        /// <summary>
        /// BottomControllerのメニュー
        /// </summary>
        #region BottomControllerMenuView BindableProperty
        public static readonly BindableProperty BottomControllerMenuViewProperty =
            BindableProperty.Create(nameof(BottomControllerMenuView), typeof(View), typeof(ChatControl), null,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((ChatControl)bindable).BottomControllerMenuView = (View)newValue);
        public View BottomControllerMenuView
        {
            get { return (View)GetValue(BottomControllerMenuViewProperty); }
            set { SetValue(BottomControllerMenuViewProperty, value); }
        }
        #endregion
    }
}