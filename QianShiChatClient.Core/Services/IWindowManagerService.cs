namespace QianShiChatClient.Core.Services;

public interface IWindowManagerService
{
    void CloseAllWindow();
    void CloseChatRoomWindow(UserInfo user);
    void CloseQueryWindow();
    void CloseWindow(Window window);
    bool ContainsChatRootWindow(UserInfo user);
    void OpenChatRoomWindow(UserInfo user);
    void OpenQueryWindow();
}

