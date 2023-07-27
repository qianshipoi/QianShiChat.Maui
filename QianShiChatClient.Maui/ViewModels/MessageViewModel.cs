namespace QianShiChatClient.Maui.ViewModels;

[QueryProperty(nameof(CurrentSelectedSession), nameof(CurrentSelectedSession))]
public sealed partial class MessageViewModel : ViewModelBase
{
    private readonly ChatHub _chatHub;
    private readonly Dictionary<Session, View> _viewCache;
    private readonly WindowManagerService _windowManagerService;

    [ObservableProperty]
    private View _content;

    public DataCenter DataCenter { get; }

    [ObservableProperty]
    private Session _currentSelectedSession;

    partial void OnCurrentSelectedSessionChanged(Session value)
    {
        if (value is null)
        {
            Content = null;
            return;
        }

        if (_windowManagerService.ContainsChatRootWindow(value.User))
        {
            _windowManagerService.OpenChatRoomWindow(value.User);
            CurrentSelectedSession = null;
            return;
        }

        if (!_viewCache.TryGetValue(value, out var view))
        {
            var viewModel = ServiceHelper.GetService<ChatMessageViewModel>();
            viewModel.Session = value;
            view = new ChatMessageView(viewModel);
            view.Opacity = 0;
            view.FadeTo(1);
            _viewCache.Add(value, view);
        }

        Content = view;
    }

    [ObservableProperty]
    private ChatMessage _toMessage;

    [ObservableProperty]
    private bool _scrollAnimated;

    [ObservableProperty]
    private string _message;

    public MessageViewModel(
        DataCenter dataCenter,
        ChatHub chatHub,
        WindowManagerService windowManagerService)
    {
        DataCenter = dataCenter;
        _windowManagerService = windowManagerService;
        _chatHub = chatHub;
        _viewCache = new();
        _ = UpdateMessage();
    }

    [RelayCommand]
    private async Task Send()
    {
        if (IsBusy || string.IsNullOrEmpty(Message)) return;

        IsBusy = true;
        try
        {
            var message = await DataCenter.SendTextAsync(User, CurrentSelectedSession, Message);
            ScrollAnimated = true;
            ToMessage = message;
            Message = string.Empty;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void OpenNewWindow(Session session)
    {
        _windowManagerService.OpenChatRoomWindow(session.User);

        if(CurrentSelectedSession == session)
        {
            CurrentSelectedSession = null;
        }
    }

    [RelayCommand]
    private Task Search(string searchText)
    {
        Toast.Make("Search:" + searchText).Show();
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task JoinDetail(Session item)
    {
        if (AppConsts.IsDesktop)
        {
            CurrentSelectedSession = item;

            if (!_viewCache.TryGetValue(item, out var view))
            {
                var viewModel = ServiceHelper.GetService<ChatMessageViewModel>();
                viewModel.Session = item;
                view = new ChatMessageView(viewModel);
                _viewCache.Add(item, view);
            }

            Content = view;

            return Task.CompletedTask;
        }
        else
        {
            return _navigationService.GoToMessageDetailPage(item.User.Id);
        }
    }

    [RelayCommand]
    private Task JoinSearchPage() => _navigationService.GoToSearchPage();

    [RelayCommand]
    private Task JoinQueryPage()
    {
        if (AppConsts.IsDesktop)
        {
            return _navigationService.GoToQueryPage();
        }
        else
        {
            return _navigationService.GoToScanningPage();
        }
    }

    [RelayCommand]
    private async Task UpdateMessage()
    {
        try
        {
            await DataCenter.GetUnreadMessageAsync();
            await _chatHub.Connect();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void OpenMenu()
    {
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Locked;
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
        Shell.Current.FlyoutIsPresented = true;
    }
}