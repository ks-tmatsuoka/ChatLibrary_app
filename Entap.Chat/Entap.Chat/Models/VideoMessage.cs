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
        }
        public VideoMessage(MessageBase messageBase)
        {
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
        }

        private string mediaUrl;
        public string MediaUrl
        {
            get
            {
                return mediaUrl;
            }
            set
            {
                if (mediaUrl != value)
                {
                    mediaUrl = value;
                    Task.Run(() =>
                    {
                        // サムネイルを生成
                        // ValueConverterだとTaskで処理できないのでここで処理している
                        var source = DependencyService.Get<IVideoService>().GenerateThumbImage(mediaUrl);
                        Thumbnail = source;
                    });
                    OnPropertyChanged("MediaUrl");
                }
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
