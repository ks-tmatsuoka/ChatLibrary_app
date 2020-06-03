using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Entap.Chat
{
    [Preserve(AllMembers = true)]
    public class TextMessage : MessageBase
    {
        public TextMessage()
        {
            MessageType = (int)Chat.MessageType.Text;
            //PropertyChanged += TextMessagePropertyChanged;
            //CreateThumbnail();
        }

        public TextMessage(MessageBase messageBase)
        {
            //PropertyChanged += TextMessagePropertyChanged;
            MessageId = messageBase.MessageId;
            SendDateTime = messageBase.SendDateTime;
            Text = messageBase.Text;
            MediaUrl = messageBase.MediaUrl;
            UserIcon = messageBase.UserIcon;
            MessageType = messageBase.MessageType;
            SendUserId = messageBase.SendUserId;
            AlreadyReadCount = messageBase.AlreadyReadCount;
            ResendVisible = messageBase.ResendVisible;
            NotSendId = messageBase.NotSendId;
            DateVisible = messageBase.DateVisible;

            //CreateThumbnail();
        }

        //private void TextMessagePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == nameof(MediaUrl) && !string.IsNullOrEmpty(UserIcon))
        //    {
        //        CreateThumbnail();
        //    }
        //}

        //void CreateThumbnail()
        //{
        //    if (!string.IsNullOrEmpty(UserIcon) && SendUserId != Settings.Current.ChatService.GetUserId())
        //    {
        //        Task.Run(() =>
        //        {
        //            //サイズを落とした画像をサムネイルとして表示
        //            //ValueConverterだとTaskで処理できないのでここで処理している
        //            //var source = DependencyService.Get<IImageService>().DownSizeImage(UserIcon);
        //            UserIconThumbnail = UserIcon;
        //        });
        //    }
        //}

        //private ImageSource userIconThumbnail;
        //public ImageSource UserIconThumbnail
        //{
        //    get
        //    {
        //        return userIconThumbnail;
        //    }
        //    set
        //    {
        //        if (userIconThumbnail != value)
        //        {
        //            userIconThumbnail = value;
        //            OnPropertyChanged("UserIconThumbnail");
        //        }
        //    }
        //}
    }
}
