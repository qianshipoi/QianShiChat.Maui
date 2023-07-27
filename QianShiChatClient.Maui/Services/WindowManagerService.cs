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
                App.Current.OpenWindow(roomWindow);
                return;
            }

            var session = _dataConter.Sessions.FirstOrDefault(x => x.User.Id == user.Id);

            if (session is not null)
            {
                _dataConter.Sessions.Remove(session);
            }
            else
            {
                session = new Session(user, new List<ChatMessage>());
            }
            _dataConter.Sessions.Insert(0, session);

            var viewModel = ServiceHelper.GetService<ChatMessageViewModel>();
            viewModel.Session = session;
            var page = new ChatRoomPage();
            page.BindingContext = viewModel;
            var window = new DesktopWindow(page);
            _chatRoomWindows.Add(user.Id, window);
            App.Current.OpenWindow(window);
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
            var keys = _chatRoomWindows.Where(x => x.Value == window).Select(x => x.Key);

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
