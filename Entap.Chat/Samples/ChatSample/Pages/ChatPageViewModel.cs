using Entap.Chat;
using Xamarin.Forms;

namespace ChatSample
{
    public class ChatPageViewModel : PageViewModelBase
    {
        public ChatPageViewModel(Room room)
        {
            RoomId = room.RoomId;
            RoomType = room.RoomType;
            LastReadMessageId = room.AlreadyReadMessageId;
            //BottomControllerMenuView = new CustomBottomMenuView();
        }
        public int RoomId { get; set; }
        public int RoomType { get; set; }
        public bool IsGroupChat => (RoomType == RoomTypes.AdminGroup || RoomType == RoomTypes.UserGroup);
        public int LastReadMessageId { get; set; }
        public BottomContorollerMenuViewBase BottomControllerMenuView { get; set; }
    }
}
