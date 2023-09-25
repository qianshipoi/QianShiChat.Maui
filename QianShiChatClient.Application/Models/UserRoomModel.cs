namespace QianShiChatClient.Application.Models;

public partial class UserRoomModel : RoomModelBase
{
    public UserInfoModel User { get; init; }
    public UserRoomModel(string id, UserInfoModel user) : base(id, user.Id, ChatMessageSendType.Personal)
    {
        User = user;
        AvatarPath = user.Avatar;
    }
}
