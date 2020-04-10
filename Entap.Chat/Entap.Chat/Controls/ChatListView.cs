using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Entap.Chat
{
    /*
    [Preserve(AllMembers = true)]
    public class ChatListView : ListView
    {
        const int RemainingItemsThreshold = 5;

        ObservableCollection<MessageBase> _messages;
        //public ChatListView() : base(ListViewCachingStrategy.RecycleElement)
        public ChatListView()
        {
            Init();
        }

        void Init()
        {
            //Scrolled += OnScrolled;
            ItemAppearing += OnItemAppearing;
            //ItemDisappearing += OnItemDisappearing;
            SelectionMode = ListViewSelectionMode.None;
            SeparatorVisibility = SeparatorVisibility.None;
            HasUnevenRows = true;
            Task.Run(async() =>
            {
                var messages = await Settings.Current.Messaging.GetMessagesAsync(-1, 20);
                var last = messages?.Last();
                if (last == null) return;
                //// ToDo : 2回目以降に表示時にスクロールが無効
                
                _messages = new ObservableCollection<MessageBase>(messages);
                Device.BeginInvokeOnMainThread(async () =>
                {
                    ItemsSource = _messages;
                    ScrollTo(last, ScrollToPosition.End, false);
                    
                });
            });
        }


        bool _isRunningGetMessage = false;
        void LoadMessages(int messageId)
        {
            Task.Run(async () =>
            {
                var messages = await Settings.Current.Messaging.GetMessagesAsync(messageId, 20);
                //_messages.InsertRange(0, messages);
                foreach (var message in messages.Reverse())
                {
                    _messages.Insert(0, message);
                }
                _isRunningGetMessage = false;
            });
        }

        bool _isRunningGetNewMessage = false;
        void LoadNewMessages(int messageId)
        {
            Task.Run(async () =>
            {
                var messages = await Settings.Current.Messaging.GetNewMessagesAsync(messageId, 20);
                _messages.AddRange(messages);
                _isRunningGetNewMessage = false;
            });
        }

        int _lastAppearingIndex;
        void OnItemAppearing(object sender, ItemVisibilityEventArgs e)
        {
            if (_isRunningGetMessage) return;
            if (_isRunningGetNewMessage) return;
            System.Diagnostics.Debug.WriteLine("OnItemAppearing  index :" + e.ItemIndex);
            ScrollDirection direction;
            if (e.ItemIndex < _lastAppearingIndex)
                direction = ScrollDirection.Up;
            else if (e.ItemIndex > _lastAppearingIndex)
                direction = ScrollDirection.Down;
            else
                direction = ScrollDirection.None;

            switch (direction)
            {
                case ScrollDirection.Up:
                    if (_isRunningGetMessage) break;

                    if (_lastAppearingIndex > RemainingItemsThreshold &&
                        e.ItemIndex <= RemainingItemsThreshold)
                    {
                        _isRunningGetMessage = true;
                        System.Diagnostics.Debug.WriteLine("Reached Up: " + e.ItemIndex + "  " + _lastAppearingIndex);
                        var first = _messages.First();
                        LoadMessages(first.Id - 1);
                    }
                    break;
                case ScrollDirection.Down:
                    if (_isRunningGetNewMessage) break;

                    var thresholdIndex = _messages.Count - 1 - RemainingItemsThreshold;
                    if (_lastAppearingIndex < thresholdIndex &&
                        e.ItemIndex >= thresholdIndex)
                    {
                        _isRunningGetNewMessage = true;

                        System.Diagnostics.Debug.WriteLine("Reached Down: " + e.ItemIndex + "  " + _lastAppearingIndex);
                        var last = _messages.Last();
                        LoadNewMessages(last.Id + 1);
                    }
                    break;
            }

            _lastAppearingIndex = e.ItemIndex;
        }

        void OnItemDisappearing(object sender, ItemVisibilityEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("OnItemDisappearing  index :" + e.ItemIndex);
        }

        double _scrollY;
        void OnScrolled(object sender, ScrolledEventArgs e)
        {
            ScrollDirection direction;
            if (e.ScrollY < _scrollY)
                direction = ScrollDirection.Up;
            else if (e.ScrollY > _scrollY)
                direction = ScrollDirection.Down;
            else
                direction = ScrollDirection.None;

            System.Diagnostics.Debug.WriteLine($"direction : {direction}  x : {e.ScrollX}  y : {e.ScrollY}");

            switch (direction)
            {
                case ScrollDirection.Up:
                    if (_scrollY > RemainingItemsThreshold &&
                        e.ScrollY <= RemainingItemsThreshold)
                    {
                        System.Diagnostics.Debug.WriteLine("Reached Up: " + e.ScrollY + "  " + _scrollY);
                        var first = _messages.First();
                        LoadMessages(first.Id - 1);
                    }

                    break;
                //case ScrollDirection.Down:
                //    if (_lastVisibleItemIndex == e.LastVisibleItemIndex) break;

                //    var thresholdIndex = _messages.Count - 1 - RemainingItemsThreshold;
                //    if (_lastVisibleItemIndex < thresholdIndex &&
                //        e.LastVisibleItemIndex >= thresholdIndex)
                //    {
                //        System.Diagnostics.Debug.WriteLine("Reached Down: " + e.LastVisibleItemIndex + " " + _lastVisibleItemIndex);
                //        var last = _messages.Last();
                //        LoadNewMessages(last.Id + 1);
                //    }
                //    _lastVisibleItemIndex = e.LastVisibleItemIndex;
                //    break;
                    
            }
            _scrollY = e.ScrollY;
        }

        public bool AddMessage(MessageBase msg)
        {
            _messages.Add(msg);
            Task.Run(async() =>
            {
                await Task.Delay(500);
                Device.BeginInvokeOnMainThread(() => ScrollTo(msg, ScrollToPosition.End, false));
            });
            return true;
        }

        int _firstVisibleItemIndex = 0;
        int _lastVisibleItemIndex = 0;
        void OnScrolled(object sender, ItemsViewScrolledEventArgs e)
        {

            ScrollDirection direction;
            if (e.VerticalDelta < 0)
                direction = ScrollDirection.Up;
            else if (e.VerticalDelta > 0)
                direction = ScrollDirection.Down;
            else
                direction = ScrollDirection.None;

            switch (direction)
            {
                case ScrollDirection.Up:
                    if (_firstVisibleItemIndex == e.FirstVisibleItemIndex) break;

                    if (_firstVisibleItemIndex > RemainingItemsThreshold &&
                        e.FirstVisibleItemIndex <= RemainingItemsThreshold)
                    {
                        System.Diagnostics.Debug.WriteLine("Reached Up: " + e.FirstVisibleItemIndex + "  " + _firstVisibleItemIndex);
                        var first = _messages.First();
                        LoadMessages(first.Id - 1);
                    }
                    _firstVisibleItemIndex = e.FirstVisibleItemIndex;

                    break;
                case ScrollDirection.Down:
                    if (_lastVisibleItemIndex == e.LastVisibleItemIndex) break;

                    var thresholdIndex = _messages.Count - 1 - RemainingItemsThreshold;
                    if (_lastVisibleItemIndex < thresholdIndex &&
                        e.LastVisibleItemIndex >= thresholdIndex)
                    {
                        System.Diagnostics.Debug.WriteLine("Reached Down: " + e.LastVisibleItemIndex + " " + _lastVisibleItemIndex);
                        var last = _messages.Last();
                        LoadNewMessages(last.Id + 1);
                    }
                    _lastVisibleItemIndex = e.LastVisibleItemIndex;
                    break;
            }
        }

        enum ScrollDirection
        {
            Up,
            Down,
            None
        }
    }
    */

    [Preserve(AllMembers = true)]
    public class ChatListView : ListView
    {
        const int DefaultRemainingItemsThreshold = 7;
        const int NotSendMessageId = -1;
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
            HasUnevenRows = true;
            SelectionMode = ListViewSelectionMode.None;
            SeparatorVisibility = SeparatorVisibility.None;
            Scrolled += OnScrolled;

            if (Device.RuntimePlatform == Device.iOS)
            {
                ItemAppearing += OnItemAppearing;
                ItemDisappearing += OnItemDisappearing;
            }
        }

        void GetFirstDisplayMessage()
        {
            if (RoomId < 0 || LastReadMessageId < 0)
                return;
            lastReadMessageId = LastReadMessageId;
            if (lastReadMessageId == 0)
                lastReadMessageId = 1;
            Task.Run(async () =>
            {
                chatMembers = await Settings.Current.ChatService.GetRoomMembers(RoomId);
                var messages = await Settings.Current.ChatService.GetMessagesAsync(RoomId, lastReadMessageId, chatMembers);
                var last = messages?.Last();
                _messages = new ObservableCollection<MessageBase>(messages);
                if (_messages.Count() < DefaultRemainingItemsThreshold * 3)
                {
                    var newMessages = await Settings.Current.ChatService.GetNewMessagesAsync(RoomId, LastReadMessageId, chatMembers);
                    foreach(var msg in newMessages)
                    {
                        _messages.Add(msg);
                    }
                    if (last == null)
                    {
                        newMessages?.Last();
                    }
                }
                if (last == null)
                {
                    return;
                }

                SetNotSendMessage();
                DateVisibleUpdate();
                Device.BeginInvokeOnMainThread(async () =>
                {
                    ItemsSource = _messages;
                    if (_messages.Count > 0)
                        ScrollTo(_messages?.Last(), ScrollToPosition.End, false);
                    _messages.CollectionChanged += OnMessagesCollectionChanged;
                    Settings.Current.ChatService.UpdateData(_messages);
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

                System.Diagnostics.Debug.WriteLine("msgLast.MessageId: " + msgLast.MessageId);
                System.Diagnostics.Debug.WriteLine("lastVisibleMessageBase.MessageId: " + lastVisibleMessageBase.MessageId);
                if (msgLast != null && lastVisibleMessageBase != null && (msgLast.MessageId == lastVisibleMessageBase.MessageId + 1 || lastVisibleMessageBase.MessageId == NotSendMessageId))
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
            DateTime dateTime = first.DateTime;
            Device.BeginInvokeOnMainThread(() =>
            {
                first.DateVisible = true;
            });
            foreach (var msg in _messages)
            {
                if (first.Equals(msg))
                    continue;
                if (dateTime.ToString("yyyy/MM/dd") == msg.DateTime.ToString("yyyy/MM/dd"))
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
                dateTime = msg.DateTime;
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
                var messages = await Settings.Current.ChatService.GetMessagesAsync(RoomId, messageId, chatMembers);

                Device.BeginInvokeOnMainThread(() =>
                {
                    IsEnabled = false;
                    foreach (var msg in messages.Reverse())
                    {
                        _messages.Insert(0, msg);
                    }
                    if (Device.RuntimePlatform == Device.Android)
                        ScrollTo(firstVisibleItem, ScrollToPosition.Start, false);
                    IsEnabled = true;
                    // firstVisibleItemIndexを一度ありえない値にしておかないとどんどん前のデータの読み込みが行われる
                    firstVisibleItemIndex = -1;
                    IsRunningGetOldMessage = false;
                    DateVisibleUpdate();
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
                var messages = await Settings.Current.ChatService.GetNewMessagesAsync(RoomId, messageId, chatMembers);
                Device.BeginInvokeOnMainThread(() =>
                {
                    _messages.AddRange(messages);
                    IsRunningGetNewMessage = false;
                    ReplaceNotSendMessage(false);
                    DateVisibleUpdate();
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
                //System.Diagnostics.Debug.WriteLine("OnItemAppearing firstVisibleItem" + ((MessageBase)e.Item).MessageId.ToString());
                firstVisibleItem = e.Item;
            }
            else if (chatScrollDirection == ScrollDirection.Down)
            {
                lastVisibleItemIndex = e.ItemIndex;
                System.Diagnostics.Debug.WriteLine("OnItemAppearing" + ((MessageBase)e.Item).MessageId.ToString());
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
            System.Diagnostics.Debug.WriteLine("firstVisibleItem" + ((MessageBase)firstVisibleItem).MessageId.ToString());
            System.Diagnostics.Debug.WriteLine("lastVisibleItem" + ((MessageBase)lastVisibleItem).MessageId.ToString());

            SendAlreadyRead(lastVisibleItem);
        }

        /// <summary>
        /// 既読済みメッセージをAPIに送る
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        async Task SendAlreadyRead(object obj)
        {
            var messageBase = obj as MessageBase;
            if (messageBase != null && lastReadMessageId < messageBase.MessageId)
            {
                System.Diagnostics.Debug.WriteLine("SendAlreadyRead: " + messageBase.MessageId);
                var result = await Settings.Current.ChatService.SendAlreadyRead(messageBase.MessageId);
                if (result == 0)
                {
                    lastReadMessageId = messageBase.MessageId;
                }
            }
            
        }

        double lastScrollY = 0;
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
                var last = _messages.Where(w=>w.NotSendId < 1)?.Last();
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
            if (Device.RuntimePlatform == Device.Android)
            {
                var dummy = new MessageBase() { SendUserId= Settings.Current.ChatService.GetUserId(), MessageType=2};
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
        /// 未送信メッセージに指定するMessageIdを取得
        /// </summary>
        /// <returns></returns>
        public int GetNotSendMessageId()
        {
            return NotSendMessageId;
        }

        /// <summary>
        /// 未送信メッセージをストレージに保存
        /// </summary>
        /// <param name="messageBase"></param>
        public void NotSendMessageSaveInStorage(MessageBase messageBase)
        {
            if (messageBase.NotSendId < 1)
            {
                Settings.Current.ChatService.SaveNotSendMessageData(RoomId, messageBase);
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

        enum ScrollDirection
        {
            Up,
            Down,
            None
        }
    }
}
