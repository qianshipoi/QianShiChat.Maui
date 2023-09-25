namespace QianShiChatClient.Application.Models;

public partial class GroupRoomModel : RoomModelBase
{
    public GroupModel Group { get; init; }
    public GroupRoomModel(string id, GroupModel group) : base(id, group.Id, ChatMessageSendType.Group)
    {
        Group = group;
        AvatarPath = group.Avatar;
    }
}
