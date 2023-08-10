namespace QianShiChatClient.Maui.ViewModels;

[QueryProperty(nameof(CurrentSelectedSession), nameof(CurrentSelectedSession))]
public sealed partial class MessageViewModel : ViewModelBase
{
    private readonly ChatHub _chatHub;
    private readonly Dictionary<SessionModel, View> _viewCache;
    private readonly ILogger<MessageViewModel> _logger;

    [ObservableProperty]
    private View _content;

    public DataCenter DataCenter { get; }

    [ObservableProperty]
    private SessionModel _currentSelectedSession;

    partial void OnCurrentSelectedSessionChanged(SessionModel value)
    {
        if (value is null)
        {
            Content = null;
            return;
        }

        var windowManagerService =  ServiceHelper.GetService<IWindowManagerService>();
        if(windowManagerService != null)
        {
            if (windowManagerService.ContainsChatRootWindow(value.User))
            {
                windowManagerService.OpenChatRoomWindow(value.User);
                CurrentSelectedSession = null;
                return;
            }
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
    private ChatMessageModel _toMessage;

    [ObservableProperty]
    private bool _scrollAnimated;

    [ObservableProperty]
    private string _message;

    public MessageViewModel(
        DataCenter dataCenter,
        ChatHub chatHub,
        ILogger<MessageViewModel> logger)
    {
        DataCenter = dataCenter;
        _chatHub = chatHub;
        _viewCache = new();
        _ = UpdateMessage();
        _logger = logger;
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "send message error.");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void OpenNewWindow(SessionModel session)
    {
        ServiceHelper.GetService<IWindowManagerService>().OpenChatRoomWindow(session.User);
        if (CurrentSelectedSession == session)
        {
            CurrentSelectedSession = null;
        }
    }

    [RelayCommand]
    private Task Search(string searchText)
    {
        return Toast.Make("Search:" + searchText).Show();
    }

    [RelayCommand]
    private Task JoinDetail(SessionModel item)
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
            ServiceHelper.GetService<IWindowManagerService>()?.OpenQueryWindow();
            return Task.CompletedTask;
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "update messages error.");
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