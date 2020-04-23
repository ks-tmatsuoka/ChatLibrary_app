using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Entap.Chat
{
    [Preserve(AllMembers = true)]
    public class VideoMessage : MessageBase
    {
        public VideoMessage()
        {
            PropertyChanged += VideoMessagePropertyChanged;
            CreateThumbnail();
        }

        public VideoMessage(MessageBase messageBase)
        {
            PropertyChanged += VideoMessagePropertyChanged;

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

        private void VideoMessagePropertyChanged(object sender, PropertyChangedEventArgs e)
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
                    // サムネイルを生成
                    // ValueConverterだとTaskで処理できないのでここで処理している
                    var source = DependencyService.Get<IVideoService>().GenerateThumbImage(MediaUrl);
                    Thumbnail = source;
                });
            }
        }

        private ImageSource thumbnail;
        public ImageSource  Thumbnail
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
