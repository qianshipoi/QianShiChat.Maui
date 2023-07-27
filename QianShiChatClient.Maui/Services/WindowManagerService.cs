#if WINDOWS
using Microsoft.Maui.Platform;

using PInvoke;

using static PInvoke.User32;

using Microsoftui = Microsoft.UI;
using MicrosoftuiWindowing = Microsoft.UI.Windowing;
using MicrosoftuiXaml = Microsoft.UI.Xaml;
using MicrosoftuixmlMedia = Microsoft.UI.Xaml.Media;
#endif

using QianShiChatClient.Maui.Windows;

namespace QianShiChatClient.Maui.Services
{
    public class WindowManagerService
    {
        private readonly DataCenter _dataConter;

        private Dictionary<int, DesktopWindow> _chatRoomWindows;

        public WindowManagerService(DataCenter dataConter)
        {
            _dataConter = dataConter;
            _chatRoomWindows = new Dictionary<int, DesktopWindow>();
        }

        public void OpenChatRoomWindow(UserInfo user)
        {
            if (_chatRoomWindows.ContainsKey(user.Id))
            {
                var roomWindow = _chatRoomWindows[user.Id];
#if WINDOWS
                if (roomWindow.Handler?.PlatformView is not MicrosoftuiXaml.Window winuiWindow)
                    return;
                winuiWindow.Activate();
#endif
                return;
            }

            var session = _dataConter.Sessions.FirstOrDefault(x => x.User.Id == user.Id);

            if (session is null)
            {
                session = new Session(user, new List<ChatMessage>());
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

            var viewModel = ServiceHelper.GetService<ChatMessageViewModel>();
            viewModel.Session = session;
            var page = new ChatRoomPage();
            page.BindingContext = viewModel;
            var window = new DesktopWindow(page);
            _chatRoomWindows.Add(user.Id, window);
            App.Current.OpenWindow(window);
        }

        public bool ContainsChatRootWindow(UserInfo user)
        {
            return _chatRoomWindows.ContainsKey(user.Id);
        }

        public void CloseChatRoomWindow(UserInfo user)
        {
            if (!_chatRoomWindows.ContainsKey(user.Id))
            {
                return;
            }
            var roomWindow = _chatRoomWindows[user.Id];
            _chatRoomWindows.Remove(user.Id);
            App.Current.CloseWindow(roomWindow);
        }

        public void CloseAllWindow()
        {
            foreach (var window in _chatRoomWindows.Values)
            {
                App.Current.CloseWindow(window);
            }
        }

        public void CloseWindow(Window window)
        {
            var keys = _chatRoomWindows.Where(x => x.Value.Equals(window)).Select(x => x.Key);

            if (keys.Any())
            {
                foreach (var key in keys)
                {
                    _chatRoomWindows.Remove(key);
                }
                App.Current.CloseWindow(window);
            }

        }
    }
}
