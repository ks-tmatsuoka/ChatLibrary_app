using System;
using System.ComponentModel;
using Xamarin.Forms.Internals;

namespace Entap.Chat
{
    [Preserve(AllMembers = true)]
    /// <summary>
    /// メッセージ
    /// </summary>
    public class MessageBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public MessageBase()
        {
        }

        public MessageBase(MessageBase notSendMessage)
        {
            MessageId = notSendMessage.MessageId;
            SendDateTime = notSendMessage.SendDateTime;
            Text = notSendMessage.Text;
            MediaUrl = notSendMessage.MediaUrl;
            UserIcon = notSendMessage.UserIcon;
            MessageType = notSendMessage.MessageType;
            SendUserId = notSendMessage.SendUserId;
            AlreadyReadCount = 0;
            ResendVisible = true;
            //NotSendId = notSendMessage.Id;
        }

        private int messageId;
        public int MessageId
        {
            get
            {
                return messageId;
            }
            set
            {
                if (messageId != value)
                {
                    messageId = value;
                    OnPropertyChanged("MessageId");
                }
            }
        }

        private DateTime sendDateTime;
        public DateTime SendDateTime
        {
            get
            {
                return sendDateTime;
            }
            set
            {
                if (sendDateTime != value)
                {
                    sendDateTime = value;
                    OnPropertyChanged("SendDateTime");
                }
            }
        }

        public string Text { get; set; }

        //public string MediaUrl { get; set; }
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
                    OnPropertyChanged("MediaUrl");
                }
            }
        }

        private string userIcon;
        public string UserIcon
        {
            get
            {
                return userIcon;
            }
            set
            {
                if (userIcon != value)
                {
                    userIcon = value;
                    OnPropertyChanged("UserIcon");
                }
            }
        }

        /// <summary>
        /// 1:テキスト, 2:画像, 3:動画
        /// </summary>
        public int MessageType { get; set; }
        public string SendUserId { get; set; }

        private int alreadyReadCount;
        public int AlreadyReadCount
        {
            get
            {
                return alreadyReadCount;
            }
            set
            {
                if (alreadyReadCount != value)
                {
                    alreadyReadCount = value;
                    OnPropertyChanged("AlreadyReadCount");
                }
            }
        }

        private bool resendVisible;
        public bool ResendVisible
        {
            get
            {
                return resendVisible;
            }
            set
            {
                if (resendVisible != value)
                {
                    resendVisible = value;
                    OnPropertyChanged("ResendVisible");
                }
            }
        }

        public int NotSendId { get; set; }

        private bool dateVisible;
        public bool DateVisible
        {
            get
            {
                return dateVisible;
            }
            set
            {
                if (dateVisible != value)
                {
                    dateVisible = value;
                    OnPropertyChanged("DateVisible");
                }
            }
        }
    }
}
