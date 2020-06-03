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
            MessageType = (int)Chat.MessageType.Video;
            PropertyChanged += VideoMessagePropertyChanged;
            //CreateMediaThumbnail();
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
            MediaThumbnailUrl = messageBase.MediaThumbnailUrl;
            //CreateMediaThumbnail();
        }

        private void VideoMessagePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            //if (e.PropertyName == nameof(MediaUrl) && !string.IsNullOrEmpty(MediaUrl))
            //{
            //    CreateMediaThumbnail();
            //}
        }

        //bool RunningCreateMediaThumbnail = false;
        //void CreateMediaThumbnail()
        //{
        //    if (!string.IsNullOrEmpty(MediaUrl) && !RunningCreateMediaThumbnail)
        //    {
        //        RunningCreateMediaThumbnail = true;
        //        System.Diagnostics.Debug.WriteLine("Create start Thumbnail:" + MediaUrl);
        //        Task.Run(() =>
        //        {
        //            // サムネイルを生成
        //            // ValueConverterだとTaskで処理できないのでここで処理している
        //            var source = DependencyService.Get<IVideoService>().GenerateThumbImage(MediaUrl);
        //            MediaThumbnail = source;
        //            System.Diagnostics.Debug.WriteLine("Create end Thumbnail:" + MediaUrl);
        //            RunningCreateMediaThumbnail = false;
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
    }
}
