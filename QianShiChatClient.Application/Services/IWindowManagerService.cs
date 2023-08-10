namespace QianShiChatClient.Application.Services;

public interface IWindowManagerService
{
    void CloseAllWindow();
    void CloseChatRoomWindow(UserInfoModel user);
    void CloseQueryWindow();
    void CloseWindow(Window window);
    bool ContainsChatRootWindow(UserInfoModel user);
    void OpenChatRoomWindow(UserInfoModel user);
    void OpenQueryWindow();
}

