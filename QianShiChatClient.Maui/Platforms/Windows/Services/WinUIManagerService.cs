using QianShiChatClient.Application.Services;
using QianShiChatClient.Maui.Windows;

using MicrosoftuiXaml = Microsoft.UI.Xaml;

namespace QianShiChatClient.Maui.Services;

public class WinUIManagerService : IWindowManagerService
{
    private const string QueryWindowId = "query";
    private readonly DataCenter _dataConter;
    private readonly IUserService _userService;

    private Dictionary<string, DesktopWindow> _opendWindows;

    public WinUIManagerService(DataCenter dataConter, IUserService userService)
    {
        _dataConter = dataConter;
        _opendWindows = new();
        _userService = userService;
    }

    private string GetUserWindowId(UserInfoModel user) => $"user_{user.Id}";

    public void OpenChatRoomWindow(UserInfoModel user)
    {
        var windowId = GetUserWindowId(user);
        if (_opendWindows.ContainsKey(windowId))
        {
            var roomWindow = _opendWindows[windowId];
            if (roomWindow.Handler?.PlatformView is not MicrosoftuiXaml.Window winuiWindow)
                return;
            winuiWindow.Activate();
            return;
        }

        var session = _dataConter.Sessions.FirstOrDefault(x => x.User.Id == user.Id);

        if (session is null)
        {
            session = new SessionModel(_userService, user, new List<ChatMessageModel>());
            _dataConter.Sessions.Insert(0, session);
        }
        else
        {
            var index = _dataConter.Sessions.IndexOf(session);
            if (index > 0)
            {
                _dataConter.Sessions.Move(index, 0);
            }
        }

        var viewModel = App.Current.ServiceProvider.GetService<ChatMessageViewModel>();
        viewModel.Session = session;
        var page = new ChatRoomPage();
        page.BindingContext = viewModel;
        var window = new DesktopWindow(windowId, page);
        _opendWindows.Add(windowId, window);
        App.Current.OpenWindow(window);
    }

    public bool ContainsChatRootWindow(UserInfoModel user)
    {
        return _opendWindows.ContainsKey(GetUserWindowId(user));
    }

    public void CloseChatRoomWindow(UserInfoModel user) => CloseWindow(GetUserWindowId(user));

    public void CloseWindow(string windowId)
    {
        if (!_opendWindows.ContainsKey(windowId))
        {
            return;
        }
        var roomWindow = _opendWindows[windowId];
        _opendWindows.Remove(windowId);
        App.Current.CloseWindow(roomWindow);
    }

    public void OpenQueryWindow()
    {
        if (_opendWindows.ContainsKey(QueryWindowId))
        {
            var roomWindow = _opendWindows[QueryWindowId];
            if (roomWindow.Handler?.PlatformView is not MicrosoftuiXaml.Window winuiWindow)
                return;
            winuiWindow.Activate();
            return;
        }

        var page = App.Current.ServiceProvider.GetService<DesktopAddQueryPage>();
        var window = new DesktopWindow(QueryWindowId, page);
        window.Height = 400;
        window.Width = 320;
        _opendWindows.Add(QueryWindowId, window);
        App.Current.OpenWindow(window);
        window.NoResize();
    }

    public void CloseQueryWindow() => CloseWindow(QueryWindowId);

    public void CloseAllWindow()
    {
        foreach (var window in _opendWindows.Values)
        {
            App.Current.CloseWindow(window);
        }
    }
}
