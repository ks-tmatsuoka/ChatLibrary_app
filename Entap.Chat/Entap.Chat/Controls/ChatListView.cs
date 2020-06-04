using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Entap.Chat.Modules;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Entap.Chat
{
    [Preserve(AllMembers = true)]
    public class ChatListView : ListView
    {
        const int DefaultRemainingItemsThreshold = 7;
        public static int NotSendMessageId = -1;
        int lastReadMessageId;
        ObservableCollection<MessageBase> _messages;
        List<ChatMemberBase> chatMembers = new List<ChatMemberBase>();
        public ObservableCollection<MessageBase> Messages => _messages;

        public ChatListView() : base(ListViewCachingStrategy.RecycleElement)
        {
            Init();
        }

        public ChatListView(ListViewCachingStrategy cachingStrategy) : base(cachingStrategy)
        {
            Init();
        }

        void Init()
        {
            if (Entap.Chat.Settings.Current.ChatService is null)
                throw new ArgumentNullException($"{typeof(Settings).FullName}.{nameof(Settings.ChatService)}");

            HasUnevenRows = true;
            SelectionMode = ListViewSelectionMode.None;
            SeparatorVisibility = SeparatorVisibility.None;
            Scrolled += OnScrolled;
            AddMessageCommand = new Command(()=>
            {
                AddMessage(AddMessageCommandParameter);
            });
            if (Device.RuntimePlatform == Device.iOS)
            {
                ItemAppearing += OnItemAppearing;
                ItemDisappearing += OnItemDisappearing;
            }
        }

        public void Dispose()
        {
            FileManager.ClearDirectory(Settings.Current.ChatService.GetSendImageSaveFolderPath());
            FileManager.ClearDirectory(Settings.Current.ChatService.GetSendVideoSaveFolderPath());
            Settings.Current.ChatService.Dispose();
        }

        // ItemsSourceにデータ入れた後Task.Delayを挟まないとScrollToがうまく機能しない
        // またTask.Delayした際どうしてもScrollする前のデータが一瞬現れてしまう
        // そのためOpacity=0で消しておいて、Scroll終わってからOpacityを戻してリストを表示している
        void GetFirstDisplayMessage()
        {
            if (RoomId < 0 || LastReadMessageId < 0)
                return;
            Opacity = 0;
            lastReadMessageId = LastReadMessageId;
            Task.Run(async () =>
            {
                //chatMembers = await Settings.Current.ChatService.GetRoomMembers(RoomId);
                IEnumerable<MessageBase> messages;
                if (lastReadMessageId == 0)
                {
                    messages = await Settings.Current.ChatService.GetMessagesAsync(RoomId, lastReadMessageId + 1, (int)MessageDirection.New, chatMembers);
                    if (messages is null)
                        messages = new List<MessageBase>();
                }
                else
                {
                    messages = await Settings.Current.ChatService.GetMessagesAsync(RoomId, lastReadMessageId, (int)MessageDirection.Old, chatMembers);
                    if (messages is null)
                        messages = new List<MessageBase>();
                    messages = messages.Reverse();
                }
                var first = messages.FirstOrDefault();
                _messages = new ObservableCollection<MessageBase>(messages);
                if (messages.Count() < 1)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        ItemsSource = _messages;
                        Opacity = 1;
                    });
                    return;
                }
                if (first != null)
                {
                    var messageList = await Settings.Current.ChatService.GetMessagesAsync(RoomId, first.MessageId - 1, (int)MessageDirection.Old, chatMembers);
                    if (messageList != null)
                    {
                        foreach (var msg in messageList)
                        {
                            _messages.Insert(0, msg);
                        }
                    }
                }
                var last = _messages.LastOrDefault();
                if (last != null)
                {
                    var messageList = await Settings.Current.ChatService.GetMessagesAsync(RoomId, last.MessageId + 1, (int)MessageDirection.New, chatMembers);
                    if (messageList != null)
                    {
                        foreach (var msg in messageList)
                        {
                            _messages.Add(msg);
                        }
                    } 
                }

                SetNotSendMessage();
                DateVisibleUpdate();
                Device.BeginInvokeOnMainThread(() =>
                {
                    ItemsSource = _messages;
                });
                await Task.Delay(1);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    var lastReadMessage = _messages.Where(w => w.MessageId == LastReadMessageId).LastOrDefault();
                    if (lastReadMessage != null && _messages.Count > 0)
                    {
                        var index = _messages.IndexOf(lastReadMessage);
                        if (_messages.Count - 1 <= index)
                            ScrollTo(_messages[index], ScrollToPosition.End, false);
                        else
                            ScrollTo(_messages[index + 1], ScrollToPosition.End, false);
                    }
                    _messages.CollectionChanged += OnMessagesCollectionChanged;
                    Settings.Current.ChatService.UpdateData(_messages, RoomId, chatMembers);
                    Opacity = 1;
                });
            });
        }

        /// <summary>
        /// 未送信メッセージのセット
        /// </summary>
        void SetNotSendMessage()
        {
            Settings.Current.ChatService.AddNotSendMessages(RoomId, _messages);
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == LastReadMessageIdProperty.PropertyName)
            {
                GetFirstDisplayMessage();
            }
            else if (propertyName == RoomIdProperty.PropertyName)
            {
                GetFirstDisplayMessage();
            }
        }

        private void OnMessagesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var msgLast = _messages.LastOrDefault();
                var lastVisibleMessageBase = lastVisibleItem as MessageBase;
                if (lastVisibleMessageBase is null)
                    return;

                var secondFromLastItemIndex = _messages.IndexOf(msgLast) - 1;
                MessageBase secondFromLastItem = null;
                if (secondFromLastItemIndex >= 0)
                {
                    secondFromLastItem = _messages[secondFromLastItemIndex];
                }
                //if (msgLast != null && lastVisibleMessageBase != null && (msgLast.MessageId == lastVisibleMessageBase.MessageId + 1 || lastVisibleMessageBase.MessageId == NotSendMessageId))
                if (msgLast != null && lastVisibleMessageBase != null && secondFromLastItem != null && lastVisibleMessageBase.MessageId == secondFromLastItem.MessageId)
                {
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        ScrollTo(msgLast, ScrollToPosition.End, true);
                        ReplaceNotSendMessage(true);
                        DateVisibleUpdate();
                    });
                }
                else
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        ReplaceNotSendMessage(true);
                        DateVisibleUpdate();
                    });
                }
            }
        }

        /// <summary>
        /// 日時のView更新
        /// </summary>
        void DateVisibleUpdate()
        {
            if (_messages.Count < 1)
                return;
            var first = _messages.FirstOrDefault();
            DateTime dateTime = first.SendDateTime;
            Device.BeginInvokeOnMainThread(() =>
            {
                first.DateVisible = true;
            });
            foreach (var msg in _messages)
            {
                if (first.Equals(msg))
                    continue;
                if (msg.SendDateTime.Equals(new DateTime()))
                    continue;
                if (dateTime.ToString("yyyy/MM/dd") == msg.SendDateTime.ToString("yyyy/MM/dd"))
                {
                    if (msg.DateVisible)
                    {
                        Device.BeginInvokeOnMainThread(() =>
                        {
                            msg.DateVisible = false;
                        });
                    }
                    continue;
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    msg.DateVisible = true;
                });
                dateTime = msg.SendDateTime;
            }
        }

        /// <summary>
        /// 未送信メッセージをリストの一番下にになるよう入れ替え
        /// </summary>
        /// <param name="isScrolled"></param>
        void ReplaceNotSendMessage(bool isScrolled)
        {
            var notSendList = _messages.Where(w => w.MessageId == NotSendMessageId && w.ResendVisible == true).ToList();
            int notSendCount =notSendList.Count;
            if (notSendCount < 1)
                return;

            // リストの末尾から見ていき、未送信のメッセージが送信済みのメッセージより前にある場合は置き換える
            bool replaceFlg = false;
            for(int i= notSendCount; i >= 1; i--)
            {
                if (_messages[_messages.Count - i].MessageId != NotSendMessageId)
                {
                    replaceFlg = true;
                    break;
                }
            }
            if (!replaceFlg)
                return;

            var addNotSendList = new ObservableCollection<MessageBase>();
            foreach (var notSendMsg in notSendList)
            {
                _messages.Remove(notSendMsg);
                addNotSendList.Add(notSendMsg);
            }
            _messages.AddRange(addNotSendList);
            if (isScrolled)
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    ScrollTo(_messages.LastOrDefault(), ScrollToPosition.End, true);
                });
            }
        }

        bool IsRunningGetOldMessage = false;
        /// <summary>
        /// 古いメッセージ読み込み
        /// </summary>
        /// <param name="messageId"></param>
        void LoadMessages(int messageId)
        {
            Task.Run(async () =>
            {
                var messages = await Settings.Current.ChatService.GetMessagesAsync(RoomId, messageId, (int)MessageDirection.Old, chatMembers);
                if (messages is null)
                {
                    // 通信エラー時
                    lastOldRequestMessageId = 0;
                    IsRunningGetOldMessage = false;
                    return;
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    IsEnabled = false;
                    foreach (var msg in messages)
                    {
                        _messages.Insert(0, msg);
                    }
                    if (Device.RuntimePlatform == Device.Android)
                        ScrollTo(firstVisibleItem, ScrollToPosition.Start, false);
                    IsEnabled = true;
                    // firstVisibleItemIndexを一度ありえない値にしておかないとどんどん前のデータの読み込みが行われる
                    firstVisibleItemIndex = -1;
                    DateVisibleUpdate();
                    IsRunningGetOldMessage = false;
                });
            });
        }

        bool IsRunningGetNewMessage = false;
        /// <summary>
        /// 新しいメッセージ読み込み
        /// </summary>
        /// <param name="messageId"></param>
        void LoadNewMessages(int messageId)
        {
            Task.Run(async () =>
            {
                var messages = await Settings.Current.ChatService.GetMessagesAsync(RoomId, messageId, (int)MessageDirection.New, chatMembers);
                if (messages is null)
                {
                    // 通信エラー時
                    lastNewRequestMessageId = 0;
                    IsRunningGetNewMessage = false;
                    return;
                }
                Device.BeginInvokeOnMainThread(() =>
                {
                    _messages.AddRange(messages);
                    ReplaceNotSendMessage(false);
                    DateVisibleUpdate();
                    IsRunningGetNewMessage = false;
                });
            });
        }

        int firstVisibleItemIndex;
        object firstVisibleItem;
        int lastVisibleItemIndex;
        object lastVisibleItem;
        public object LastVisibleItem
        {
            get
            {
                return lastVisibleItem;
            }
        }
        ScrollDirection chatScrollDirection;

        private void OnItemDisappearing(object sender, ItemVisibilityEventArgs e)
        {
            if (chatScrollDirection == ScrollDirection.Down)
            {
                lastVisibleItemIndex = e.ItemIndex;
                lastVisibleItem = e.Item;
            }
            else if (chatScrollDirection == ScrollDirection.Up)
            {
                firstVisibleItemIndex = e.ItemIndex;
                firstVisibleItem = e.Item;
            }
        }

        private void OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (chatScrollDirection == ScrollDirection.Up)
            {
                firstVisibleItemIndex = e.ItemIndex;
                firstVisibleItem = e.Item;
                SendAlreadyRead(firstVisibleItem);
            }
            else if (chatScrollDirection == ScrollDirection.Down)
            {
                lastVisibleItemIndex = e.ItemIndex;
                lastVisibleItem = e.Item;
                SendAlreadyRead(lastVisibleItem);
            }
        }

        /// <summary>
        /// 表示中のメッセージの一番上のメッセージと一番下のメッセージを変数へ代入(Androidで使用)
        /// </summary>
        /// <param name="firstIndex"></param>
        /// <param name="firstItem"></param>
        /// <param name="lastIndex"></param>
        /// <param name="lastItem"></param>
        public void VisibleItemUpdateForAndroid(int firstIndex, object firstItem, int lastIndex, object lastItem)
        {
            firstVisibleItemIndex = firstIndex;
            firstVisibleItem = firstItem;
            lastVisibleItemIndex = lastIndex;
            lastVisibleItem = lastItem;
            SendAlreadyRead(lastVisibleItem);
        }

        /// <summary>
        /// 既読済みメッセージをAPIに送る
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task SendAlreadyRead(object obj)
        {
            var messageBase = obj as MessageBase;
            var userId = Settings.Current.ChatService.GetUserId();
            if (messageBase != null && lastReadMessageId < messageBase.MessageId)
            {
                System.Diagnostics.Debug.WriteLine("SendAlreadyRead: " + messageBase.MessageId);
                var result = await Settings.Current.ChatService.SendAlreadyRead(RoomId, messageBase.MessageId);
                if (result)
                {
                    lastReadMessageId = messageBase.MessageId;
                }
            }
            
        }

        double lastScrollY = 0;
        int lastOldRequestMessageId;
        int lastNewRequestMessageId;
        public void OnScrolled(object sender, ScrolledEventArgs e)
        {
            if (lastScrollY > e.ScrollY)
            {
                chatScrollDirection = ScrollDirection.Up;
            }   
            else if (lastScrollY < e.ScrollY)
            {
                chatScrollDirection = ScrollDirection.Down;
            }
            else
            {
                chatScrollDirection = ScrollDirection.None;
                return;
            }

            if (
                firstVisibleItemIndex <= RemainingItemsThreshold &&
                firstVisibleItemIndex > 0 &&
                chatScrollDirection == ScrollDirection.Up &&
                !IsRunningGetOldMessage
                )
            {
                IsRunningGetOldMessage = true;
                var first = _messages.First();
                if (first == null || first.MessageId - 1 < 1 || first.MessageId - 1 == lastOldRequestMessageId)
                {
                    IsRunningGetOldMessage = false;
                    return;
                }
                lastOldRequestMessageId = first.MessageId - 1;
                LoadMessages(first.MessageId - 1);
            }
            else if (
                lastVisibleItemIndex >= _messages.Count - 1 - RemainingItemsThreshold &&
                lastVisibleItemIndex <= _messages.Count - 1 &&
                chatScrollDirection == ScrollDirection.Down &&
                !IsRunningGetNewMessage
                )
            {
                IsRunningGetNewMessage = true;
                var last = _messages.LastOrDefault();
                if (last == null || (!last.ResendVisible && last.MessageId < 0))
                {
                    IsRunningGetNewMessage = false;
                    return;
                }
                last = _messages.Where(w=>w.NotSendId < 1 && w.MessageId > 0)?.Last();
                if (last.MessageId + 1 == lastNewRequestMessageId)
                {
                    IsRunningGetNewMessage = false;
                    return;
                }
                lastNewRequestMessageId = last.MessageId + 1;
                LoadNewMessages(last.MessageId + 1);
            }

            lastScrollY = e.ScrollY;
        }

        /// <summary>
        /// メッセージをリストに追加
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool AddMessage(MessageBase msg)
        {
            if (msg is null)
                return false;
            if (msg.MessageType == (int)MessageType.Image || msg.MessageType == (int)MessageType.Video)
                msg.ProgressVisible = true;
            if (_messages.Count < 1)
            {
                // 一番最初に送るメッセージは日付を表示させる
                msg.DateVisible = true;
            }
            if (Device.RuntimePlatform == Device.Android)
            {
                var dummy = new MessageBase() { SendUserId= Settings.Current.ChatService.GetUserId(), MessageType= (int)MessageType.Image};
                _messages.Add(dummy);
                _messages.Add(msg);
                // ScrollTo(msg, ScrollToPosition.End, false) だけだと画像送信した際に追加したメッセージのViewが表示されない
                // 動き的に前にテキストのメッセージがあると、そのテキストのメッセージ分の高さしかスクロールしてくれない感じになっている
                // なので一旦ダミーの画像のView追加してスクロールして、ダミーを削除
                //ScrollTo(msg, ScrollToPosition.End, false)
                ScrollTo(msg, ScrollToPosition.Start, true);
                _messages.Remove(dummy);
                return true;
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                _messages.Add(msg);
                ScrollTo(msg, ScrollToPosition.End, true);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 未送信メッセージをストレージに保存
        /// </summary>
        /// <param name="messageBase"></param>
        public void NotSendMessageSaveInStorage(MessageBase messageBase, string fileName="")
        {
            if (messageBase.NotSendId < 1)
            {
                Settings.Current.ChatService.SaveNotSendMessageData(RoomId, messageBase, fileName);
            }
        }

        /// <summary>
        /// ストレージから未送信メッセージのデータを削除
        /// </summary>
        /// <param name="notSendMessageId"></param>
        /// <returns></returns>
        public bool NotSendMessageDeleteFromStorage(int notSendMessageId)
        {
            if (notSendMessageId > 0)
            {
                Settings.Current.ChatService.DeleteNotSendMessageData(notSendMessageId);
                return true;
            }
            return false;
        }

        /// <summary>
        /// データの読み込みを始める未表示データのしきい値
        /// 未表示のアイテム数がRemainingItemsThresholdに達した際に、データ増分をロード
        /// </summary>
        public static readonly BindableProperty RemainingItemsThresholdProperty =
            BindableProperty.Create(nameof(RemainingItemsThreshold), typeof(int), typeof(ChatListView), DefaultRemainingItemsThreshold,
                propertyChanged: (bindable, oldValue, newValue) =>
                     ((ChatListView)bindable).RemainingItemsThreshold = (int)newValue);

        public int RemainingItemsThreshold
        {
            get { return (int)GetValue(RemainingItemsThresholdProperty); }
            set { SetValue(RemainingItemsThresholdProperty, value); }
        }

        /// <summary>
        /// チャットのルームID
        /// </summary>
        public static readonly BindableProperty RoomIdProperty =
            BindableProperty.Create(nameof(RoomId), typeof(int), typeof(ChatListView), -1,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((ChatListView)bindable).RoomId = (int)newValue);
        public int RoomId
        {
            get { return (int)GetValue(RoomIdProperty); }
            set { SetValue(RoomIdProperty, value); }
        }

        ///// <summary>
        ///// グループチャットか
        ///// </summary>
        public static readonly BindableProperty IsGroupChatProperty = BindableProperty.Create(
            nameof(IsGroupChat),
            typeof(bool),
            typeof(ChatListView),
            false,
            defaultBindingMode: BindingMode.TwoWay,
            propertyChanged: (bindable, oldValue, newValue) =>
            ((ChatListView)bindable).IsGroupChat = (bool)newValue);
        public bool IsGroupChat
        {
            get { return (bool)GetValue(IsGroupChatProperty); }
            set { SetValue(IsGroupChatProperty, value); }
        }

        /// <summary>
        /// 最後に既読にしたメッセージID
        /// </summary>
        public static readonly BindableProperty LastReadMessageIdProperty =
            BindableProperty.Create(nameof(LastReadMessageId), typeof(int), typeof(ChatListView), -1,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((ChatListView)bindable).LastReadMessageId = (int)newValue);
        public int LastReadMessageId
        {
            get { return (int)GetValue(LastReadMessageIdProperty); }
            set { SetValue(LastReadMessageIdProperty, value); }
        }

        /// <summary>
        /// AddMessageメソッドをViewModelから呼ぶためのコマンド
        /// </summary>
        public static readonly BindableProperty AddMessageCommandProperty =
            BindableProperty.Create(nameof(AddMessageCommand), typeof(ICommand), typeof(ChatListView), null,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((ChatListView)bindable).AddMessageCommand = (ICommand)newValue);

        public ICommand AddMessageCommand
        {
            get { return (ICommand)GetValue(AddMessageCommandProperty); }
            set { SetValue(AddMessageCommandProperty, value); }
        }

        public static readonly BindableProperty AddMessageCommandParameterProperty =
            BindableProperty.Create(nameof(AddMessageCommandParameter), typeof(MessageBase), typeof(ChatListView), null,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((ChatListView)bindable).AddMessageCommandParameter = (MessageBase)newValue);

        public MessageBase AddMessageCommandParameter
        {
            get { return (MessageBase)GetValue(AddMessageCommandParameterProperty); }
            set { SetValue(AddMessageCommandParameterProperty, value); }
        }

        enum ScrollDirection
        {
            Up,
            Down,
            None
        }
    }
}
