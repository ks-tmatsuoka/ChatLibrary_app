using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Entap.Chat
{
    [Preserve(AllMembers = true)]
    public class ChatListView_Seki : ListView
    {
        ObservableCollection<SekiTestModel> message;
        public ChatListView_Seki()
        {
            Init();
        }

        void SetDefaultData()
        {
            message = new ObservableCollection<SekiTestModel>();
            for (int i = 90; i <= 120; i++)
            {
                if (i % 3 == 0)
                {
                    if (i % 2 == 0)
                        message.Add(new SekiTestModel { Id = i, Text = "" + Environment.NewLine + i.ToString() + Environment.NewLine + "", BgColor = Color.Aqua });
                    else
                        message.Add(new SekiTestModel { Id = i, Text = "" + Environment.NewLine + "" + Environment.NewLine + i.ToString() + Environment.NewLine + "" + Environment.NewLine + "", BgColor = Color.Yellow });
                }
                else
                {
                    message.Add(new SekiTestModel { Id = i, Text = i.ToString() });
                }
            }
            this.ItemsSource = message;
        }

        void Init()
        {
            HasUnevenRows = true;
            SeparatorVisibility = SeparatorVisibility.None;
            Scrolled += OnScrolled; ;
            ItemAppearing += OnItemAppearing;
            this.ItemDisappearing += OnItemDisappearing;

            SetDefaultData();
        }

        bool IsRunningGetOldMessage = false;
        void GetOldMessage()
        {
            Task.Run(async () =>
            {
                var messages = await Settings.Current.Messaging.GetMessagesAsync(message);
                Device.BeginInvokeOnMainThread(() =>
                {
                    IsEnabled = false;
                    foreach (var msg in messages)
                    {
                        message.Insert(0, msg);
                    }
                    IsEnabled = true;
                    // firstVisibleItemIndexを一度ありえない値にしておかないとどんどん前のデータの読み込みが行われる
                    firstVisibleItemIndex = -1;
                    IsRunningGetOldMessage = false;
                });
            });
        }

        bool IsRunningGetNewMessage = false;
        void GetNewMessage()
        {
            Task.Run(async () =>
            {
                var messages = await Settings.Current.Messaging.GetNewMessagesAsync(message);
                Device.BeginInvokeOnMainThread(() =>
                {
                    foreach (var msg in messages)
                    {
                        message.Add(msg);
                    }
                    IsRunningGetNewMessage = false;
                });
            });
        }

        int firstVisibleItemIndex;
        object firstVisibleItem;
        int lastVisibleItemIndex;
        object lastVisibleItem;

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
            }
            else if (chatScrollDirection == ScrollDirection.Down)
            {
                lastVisibleItemIndex = e.ItemIndex;
                lastVisibleItem = e.Item;
            }
        }

        double lastScrollY = 0;
        private void OnScrolled(object sender, ScrolledEventArgs e)
        {
            if (lastScrollY > e.ScrollY)
                chatScrollDirection = ScrollDirection.Up;
            else if (lastScrollY < e.ScrollY)
                chatScrollDirection = ScrollDirection.Down;
            else
            {
                chatScrollDirection = ScrollDirection.None;
                return;
            }

            if (
                firstVisibleItemIndex <= RemainingItemsThreshold &&
                firstVisibleItemIndex >= 0 &&
                chatScrollDirection == ScrollDirection.Up &&
                !IsRunningGetOldMessage
                )
            {
                IsRunningGetOldMessage = true;
                GetOldMessage();
            }
            else if (
                lastVisibleItemIndex >= message.Count - 1 - RemainingItemsThreshold &&
                lastVisibleItemIndex <= message.Count - 1 &&
                chatScrollDirection == ScrollDirection.Down &&
                !IsRunningGetNewMessage
                )
            {
                IsRunningGetNewMessage = true;
                GetNewMessage();
            }

            lastScrollY = e.ScrollY;
        }



        /// <summary>
        /// データの読み込みを始める未表示データの値
        /// </summary>
        public static readonly BindableProperty RemainingItemsThresholdProperty =
            BindableProperty.Create(nameof(RemainingItemsThreshold), typeof(int), typeof(ChatListView_Seki), null,
                propertyChanged: (bindable, oldValue, newValue) =>
                                    ((ChatListView_Seki)bindable).RemainingItemsThreshold = (int)newValue);

        public int RemainingItemsThreshold
        {
            get { return (int)GetValue(RemainingItemsThresholdProperty); }
            set { SetValue(RemainingItemsThresholdProperty, value); }
        }

        enum ScrollDirection
        {
            Up,
            Down,
            None
        }
    }
}
