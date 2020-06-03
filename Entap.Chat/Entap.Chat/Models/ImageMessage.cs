using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Entap.Chat
{
    [Preserve(AllMembers = true)]
    public class ImageMessage : MessageBase
    {
        public ImageMessage()
        {
            MessageType = (int)Chat.MessageType.Image;
            //PropertyChanged += ImageMessagePropertyChanged;
            //CreateMediaThumbnail();
            //CreateUserIconThumbnail();
        }

        public ImageMessage(MessageBase messageBase)
        {
            //PropertyChanged += ImageMessagePropertyChanged;
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

            //CreateMediaThumbnail();
            //CreateUserIconThumbnail();
        }

        //private void ImageMessagePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == nameof(MediaUrl) && !string.IsNullOrEmpty(MediaUrl))
        //    {
        //        CreateMediaThumbnail();
        //    }
        //    else if (e.PropertyName == nameof(MediaUrl) && !string.IsNullOrEmpty(UserIcon))
        //    {
        //        CreateUserIconThumbnail();
        //    }
        //}

        //void CreateMediaThumbnail()
        //{
        //    if (!string.IsNullOrEmpty(MediaUrl))
        //    {
        //        Task.Run(() =>
        //        {
        //            // サイズを落とした画像をサムネイルとして表示
        //            // ValueConverterだとTaskで処理できないのでここで処理している
        //            //var source = DependencyService.Get<IImageService>().DownSizeImage(MediaUrl);
        //            MediaThumbnail = MediaUrl;
        //        });
        //    }
        //}

        //void CreateUserIconThumbnail()
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

        //private ImageSource mediaThumbnail;
        //public ImageSource MediaThumbnail
        //{
        //    get
        //    {
        //        return mediaThumbnail;
        //    }
        //    set
        //    {
        //        if (mediaThumbnail != value)
        //        {
        //            mediaThumbnail = value;
        //            OnPropertyChanged("MediaThumbnail");
        //        }
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
