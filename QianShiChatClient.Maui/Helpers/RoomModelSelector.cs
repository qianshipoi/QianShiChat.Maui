namespace QianShiChatClient.Maui.Helpers;

public class RoomModelSelector : DataTemplateSelector
{
    public DataTemplate GroupRoomTemplate { get; set; }
    public DataTemplate UserRoomTemplate { get; set; }

    private static DataTemplate EmptyDataTemplate = new DataTemplate();

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        if (item is not RoomModelBase room)
        {
            return EmptyDataTemplate;
        }

        if (room is UserRoomModel)
        {
            return UserRoomTemplate;
        }
        else if (room is GroupRoomModel)
        {
            return GroupRoomTemplate;
        }
        else
        {
            throw new NotSupportedException("未知房间");
        }
    }
}
