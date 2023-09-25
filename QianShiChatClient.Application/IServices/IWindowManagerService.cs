namespace QianShiChatClient.Application.IServices;

public interface IWindowManagerService
{
    void CloseAllWindow();
    void CloseChatRoomWindow(UserInfoModel user);
    void CloseQueryWindow();
    void CloseWindow(string winId);
    bool ContainsChatRootWindow(UserInfoModel user);
    void OpenChatRoomWindow(UserInfoModel user);
    void OpenQueryWindow();
}

