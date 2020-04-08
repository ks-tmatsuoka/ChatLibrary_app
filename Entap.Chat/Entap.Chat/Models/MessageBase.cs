using System;
using System.ComponentModel;

namespace Entap.Chat
{
    /// <summary>
    /// メッセージ
    /// </summary>
    public class MessageBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };
        public MessageBase()
        {
            DateTime = DateTime.Now;
            UserIcon = "https://brave.entap.dev/storage/user_icon.png";
        }

        public MessageBase(MessageBase notSendMessage)
        {
            MessageId = notSendMessage.MessageId;
            DateTime = notSendMessage.DateTime;
            Text = notSendMessage.Text;
            ImageUrl = notSendMessage.ImageUrl;
            UserIcon = notSendMessage.UserIcon;
            MessageType = notSendMessage.MessageType;
            SendUserId = notSendMessage.SendUserId;
            IsAlreadyRead = false;
            ResendVisible = true;
            //NotSendId = notSendMessage.Id;
        }

        public MessageType _MessageType
        {
            get
            {
                if (MessageType == 1 && SendUserId == Settings.Current.ChatService.GetUserId())
                    return Chat.MessageType.MyText;
                else if (MessageType == 2 && SendUserId == Settings.Current.ChatService.GetUserId())
                    return Chat.MessageType.MyImage;
                else if (MessageType == 2)
                    return Chat.MessageType.OthersImage;
                return Chat.MessageType.OthersText;
            }
        }

        public int MessageId { get; set; }
        public DateTime DateTime { get; set; }

        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public string UserIcon { get; set; }
        public int MessageType { get; set; }
        public string SendUserId { get; set; }

        private bool isAlreadyRead;
        public bool IsAlreadyRead
        {
            get
            {
                return isAlreadyRead;
            }
            set
            {
                if (isAlreadyRead != value)
                {
                    isAlreadyRead = value;
                    PropertyChanged(this, new PropertyChangedEventArgs("IsAlreadyRead"));
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
                    PropertyChanged(this, new PropertyChangedEventArgs("ResendVisible"));
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
                    PropertyChanged(this, new PropertyChangedEventArgs("DateVisible"));
                }
            }
        }
    }
}
