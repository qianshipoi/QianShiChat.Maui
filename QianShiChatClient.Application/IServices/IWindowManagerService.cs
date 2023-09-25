namespace QianShiChatClient.Application.IServices;

public interface IWindowManagerService
{
    void CloseAllWindow();
    void CloseChatRoomWindow(RoomModelBase room);
    void CloseQueryWindow();
    void CloseWindow(string winId);
    bool ContainsChatRootWindow(RoomModelBase room);
    void OpenChatRoomWindow(RoomModelBase room);
    void OpenQueryWindow();
}

