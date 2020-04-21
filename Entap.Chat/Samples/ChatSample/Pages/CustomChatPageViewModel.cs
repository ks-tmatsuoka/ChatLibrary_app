using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Entap.Chat;
using Newtonsoft.Json;
using System.Linq;
using Xamarin.Forms;
using ChatSample.Views;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ChatSample
{
    public class CustomChatPageViewModel : PageViewModelBase
    {
        public CustomChatPageViewModel()
        {
            // サービス管理者との1対1のルームを強制表示している
            RoomType = 1;
            RoomId = InitRoomData();
            
            var bottomMenuView = new CustomBottomMenuView();
            bottomMenuView.MenuCommand = MenuCommand;
            BottomControllerMenuView = bottomMenuView;
        }

        int InitRoomData()
        {
            var comp = new TaskCompletionSource<int>();
            Task.Run(async () =>
            {
                var json = await APIManager.PostAsync(APIManager.GetEntapAPI(APIManager.EntapAPIName.GetRoomList), new ReqGetRoomList());
                var respGetRooms = JsonConvert.DeserializeObject<RespGetRoomList>(json);
                if (respGetRooms.Status == APIManager.APIStatus.Succeeded)
                {
                    var chatService = new ChatService();
                    if (respGetRooms.Data.Rooms.Count < 1)
                    {
                        //サービス管理者とのルーム作成
                        var data = new ReqCreateRoomData()
                        {
                            UserId = UserDataManager.Instance.UserId,
                            RoomType = RoomType
                        };
                        var reqJson = JsonConvert.SerializeObject(data);
                        json = await APIManager.PostAsync(APIManager.GetEntapAPI(APIManager.EntapAPIName.CreateRoom), new ReqCreateRoom { Data = reqJson });
                        var respCreateRoom = JsonConvert.DeserializeObject<RespCreateRoom>(json);
                        if (respCreateRoom.Status == APIManager.APIStatus.Succeeded)
                        {
                            LastReadMessageId = respCreateRoom.Data.AlreadyReadMessageId;
                            comp.SetResult(respCreateRoom.Data.RoomId);
                        }
                        else
                        {
                            comp.SetResult(0);
                        }
                    }
                    else
                    {
                        var room = respGetRooms.Data.Rooms.Where(w => w.RoomType == RoomType).LastOrDefault();
                        if (room != null)
                        {
                            LastReadMessageId = room.AlreadyReadMessageId;
                            comp.SetResult(room.RoomId);
                        }
                        else
                        {
                            comp.SetResult(0);
                        }
                    }
                }
                else
                {
                    comp.SetResult(0);
                }
                
            });

            return comp.Task.Result;
        }

        async Task SendMessage(MessageBase msg = null)
        {
            if (msg is null)
            {
                if (string.IsNullOrEmpty(EditorText))
                    return;
                msg = new MessageBase { MessageId = ChatListView.NotSendMessageId, Text = EditorText, AlreadyReadCount = 0, MessageType = (int)MessageType.Text, SendUserId = Settings.Current.ChatService.GetUserId() };
                EditorText = "";
            }
            else
            {
                var oldMsgIndex = ItemsSource.IndexOf(msg);
                ItemsSource.RemoveAt(oldMsgIndex);
                msg.ResendVisible = false;
            }
            AddMessageCommandParameter = msg;
            AddMessageCommand?.Execute(AddMessageCommandParameter);

            var sendMessageResponseBase = await Settings.Current.ChatControlService.SendMessage(RoomId, msg, ChatListView.NotSendMessageId);
            var index = ItemsSource.IndexOf(msg);
            if (sendMessageResponseBase.MessageId < 0)
            {
                // 通信エラー
                ItemsSource[index].ResendVisible = true;
                if (ItemsSource[index].NotSendId < 1)
                {
                    Settings.Current.ChatService.SaveNotSendMessageData(RoomId, ItemsSource[index]);
                }
            }
            else
            {
                msg.ResendVisible = false;
                // サーバへ送信できた段階でメッセージの表示位置を再確認
                if (index == ItemsSource.Count - 1)
                {
                    ItemsSource[index].MessageId = sendMessageResponseBase.MessageId;
                    ItemsSource[index].SendDateTime = sendMessageResponseBase.SendDateTime;
                }
                else
                {
                    ItemsSource.RemoveAt(index);
                    msg.MessageId = sendMessageResponseBase.MessageId;
                    msg.SendDateTime = sendMessageResponseBase.SendDateTime;
                    ItemsSource.Add(msg);
                }
                if (msg.NotSendId > 0)
                {
                    Settings.Current.ChatService.DeleteNotSendMessageData(msg.NotSendId);
                }
            }
        }

        async Task SendLibraryImage()
        {
            var mg = new MediaPluginManager();
            var paths = await mg.PickPhotoAsyncGetPathAndAlbumPath();
            if (paths is null)
                return;
            byte[] bytes = null;
            string extension = "";
            string imgPath = "";
            if (!string.IsNullOrEmpty(paths[0]))
            {
                imgPath = paths[0];
                bytes = FileManager.ReadBytes(imgPath);
                if ((bytes == null || bytes.Length < 1) && paths.Count > 1)
                {
                    imgPath = paths[1];
                    bytes = FileManager.ReadBytes(imgPath);
                    extension = System.IO.Path.GetExtension(imgPath);
                }
            }

            if (bytes == null || bytes.Length < 1)
            {
                if (bytes != null && bytes.Length < 1)
                    await App.Current.MainPage.DisplayAlert(null, "こちらの画像は送信できません", "閉じる");
                return;
            }

            extension = System.IO.Path.GetExtension(imgPath);
            var copyImgPath = Settings.Current.ChatService.GetSendImageSaveFolderPath() + Guid.NewGuid() + extension;
            if (!FileManager.FileCopy(imgPath, copyImgPath))
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Application.Current.MainPage.DisplayAlert("", "画像の取得に失敗しました", "閉じる");
                });
                return;
            }
            var msg = new MessageBase { MessageId = ChatListView.NotSendMessageId, MediaUrl = copyImgPath, MessageType = (int)MessageType.Image, SendUserId = Settings.Current.ChatService.GetUserId() };
            await ChatAddImg(msg);
        }

        async Task ChatAddImg(MessageBase msg)
        {
            AddMessageCommandParameter = msg;
            AddMessageCommand?.Execute(AddMessageCommandParameter);

            var sendMessageResponseBase = await Settings.Current.ChatControlService.SendMessage(RoomId, msg, ChatListView.NotSendMessageId);
            var index = ItemsSource.IndexOf(msg);
            if (sendMessageResponseBase.MessageId < 0)
            {
                // 通信エラー
                var delImgPath = msg.MediaUrl;
                string delImgExtension = System.IO.Path.GetExtension(delImgPath);
                var target = ItemsSource[index];
                target.ResendVisible = true;
                var sendErrorImgPath = Settings.Current.ChatControlService.GetNotSendImageSaveFolderPath() + Guid.NewGuid() + delImgExtension;
                FileManager.FileCopy(delImgPath, sendErrorImgPath);
                target.MediaUrl = sendErrorImgPath;
                FileManager.FileDelete(delImgPath);
                if (target.NotSendId < 1)
                {
                    Settings.Current.ChatService.SaveNotSendMessageData(RoomId, target);
                }
            }
            else
            {
                msg.ResendVisible = false;
                // サーバへ送信できた段階でメッセージの表示位置を再確認
                if (index == ItemsSource.Count - 1)
                {
                    ItemsSource[index].MessageId = sendMessageResponseBase.MessageId;
                    ItemsSource[index].SendDateTime = sendMessageResponseBase.SendDateTime;
                }
                else
                {
                    ItemsSource.RemoveAt(index);
                    msg.MessageId = sendMessageResponseBase.MessageId;
                    msg.SendDateTime = sendMessageResponseBase.SendDateTime;
                    ItemsSource.Add(msg);
                }

                if (msg.NotSendId > 0)
                {
                    Settings.Current.ChatService.DeleteNotSendMessageData(msg.NotSendId);
                }
            }
        }

        public Command ResendCommand => new Command(async(obj) =>
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
                else if (msg.MessageType == (int)MessageType.Image)
                {
                    var oldMsgIndex = ItemsSource.IndexOf(msg);
                    ItemsSource.RemoveAt(oldMsgIndex);
                    msg.ResendVisible = false;
                    await ChatAddImg(msg);
                }
            }
            else if (result.Equals(button[1]))
            {
                if (msg.NotSendId > 0)
                {
                    Settings.Current.ChatService.DeleteNotSendMessageData(msg.NotSendId);
                    ItemsSource.Remove(msg);
                }
            }
        });
        public Command ImageShareCommand => new Command(async(obj) =>
        {
            var imagePath = obj as string;
            await ImageManager.ImageShare(imagePath);
        });
        public Command ImageTapCommand => new Command((obj) =>
        {
            var imagePath = obj as string;
            App.Current.MainPage.Navigation.PushModalAsync(new ImagePreviewPage(imagePath));
        });
        public Command SendCommand => new Command(async(obj) =>
        {
            await SendMessage();
        });
        
        public Command MenuCommand => new Command(async(obj) =>
        {
            if (int.Parse(obj.ToString()) == (int)BottomControllerMenuType.Library)
            {
                SendLibraryImage();
            }
        });

        private int roomId;
        public int RoomId
        {
            get
            {
                return roomId;
            }
            set
            {
                if (roomId != value)
                {
                    roomId = value;
                    OnPropertyChanged("RoomId");
                }
            }
        }

        private int roomType;
        public int RoomType
        {
            get
            {
                return roomType;
            }
            set
            {
                if (roomType != value)
                {
                    roomType = value;
                    OnPropertyChanged("RoomType");
                }
            }
        }

        private int lastReadMessageId;
        public int LastReadMessageId
        {
            get
            {
                return lastReadMessageId;
            }
            set
            {
                if (lastReadMessageId != value)
                {
                    lastReadMessageId = value;
                    OnPropertyChanged("LastReadMessageId");
                }
            }
        }

        private View bottomControllerMenuView;
        public View BottomControllerMenuView
        {
            get
            {
                return bottomControllerMenuView;
            }
            set
            {
                if (bottomControllerMenuView != value)
                {
                    bottomControllerMenuView = value;
                    OnPropertyChanged("BottomControllerMenuView");
                }
            }
        }

        private string editorText;
        public string EditorText
        {
            get
            {
                return editorText;
            }
            set
            {
                if (editorText != value)
                {
                    editorText = value;
                    OnPropertyChanged("EditorText");
                }
            }
        }

        private ObservableCollection<MessageBase> itemsSource;
        public ObservableCollection<MessageBase> ItemsSource
        {
            get
            {
                return itemsSource;
            }
            set
            {
                if (itemsSource != value)
                {
                    itemsSource = value;
                    OnPropertyChanged("ItemsSource");
                }
            }
        }

        ICommand addMessageCommand;
        public ICommand AddMessageCommand
        {
            get
            {
                return addMessageCommand;
            }
            set
            {
                SetProperty(ref addMessageCommand, value);
            }
        }

        private MessageBase addMessageCommandParameter;
        public MessageBase AddMessageCommandParameter
        {
            get
            {
                return addMessageCommandParameter;
            }
            set
            {
                if (addMessageCommandParameter != value)
                {
                    addMessageCommandParameter = value;
                    OnPropertyChanged("AddMessageCommandParameter");
                }
            }
        }
    }
}
