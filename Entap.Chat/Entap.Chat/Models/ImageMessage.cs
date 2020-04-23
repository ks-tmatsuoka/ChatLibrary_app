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
            PropertyChanged += ImageMessagePropertyChanged;
            CreateThumbnail();
        }

        public ImageMessage(MessageBase messageBase)
        {
            PropertyChanged += ImageMessagePropertyChanged;
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

            CreateThumbnail();
        }

        private void ImageMessagePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MediaUrl) && !string.IsNullOrEmpty(MediaUrl))
            {
                CreateThumbnail();
            }
        }

        void CreateThumbnail()
        {
            if (!string.IsNullOrEmpty(MediaUrl))
            {
                Task.Run(() =>
                {
                    // サイズを落とした画像をサムネイルとして表示
                    // ValueConverterだとTaskで処理できないのでここで処理している
                    //var source = DependencyService.Get<IImageService>().DownSizeImage(MediaUrl);
                    Thumbnail = MediaUrl;
                });
            }
        }

        private ImageSource thumbnail;
        public ImageSource Thumbnail
        {
            get
            {
                return thumbnail;
            }
            set
            {
                if (thumbnail != value)
                {
                    thumbnail = value;
                    OnPropertyChanged("Thumbnail");
                }
            }
        }
    }
}
