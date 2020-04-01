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
        public MessageBase(MessageType messageType = MessageType.None)
        {
            MessageType = messageType;
            DateTime = DateTime.Now;
        }
        public MessageType MessageType { get; private set; }
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        
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
    }
}
