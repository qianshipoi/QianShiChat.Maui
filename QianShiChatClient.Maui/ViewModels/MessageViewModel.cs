namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class MessageViewModel : ViewModelBase
{
    private readonly ChatHub _chatHub;
    private readonly IApiClient _apiClient;
    private readonly ChatDatabase _database;
    private readonly IUserService _userService;
    private readonly IServiceProvider _serviceProvider;
    private readonly Dictionary<Session, View> _viewCache;

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

        if (!_viewCache.TryGetValue(value, out var view))
        {
            var viewModel = _serviceProvider.GetRequiredService<ChatMessageViewModel>();
            viewModel.Session = value;
            view = new ChatMessageView(viewModel);
            view.Opacity = 0;
            view.FadeTo(1, 1000);
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

    public List<string> Strings { get; } = new List<string> { "111", "222", "333" };

    public MessageViewModel(
        DataCenter dataCenter,
        ChatHub chatHub,
        IApiClient apiClient,
        ChatDatabase database,
        IUserService userService,
        IServiceProvider serviceProvider)
    {
        DataCenter = dataCenter;
        _chatHub = chatHub;
        _viewCache = new();
        _ = UpdateMessage();
        _apiClient = apiClient;
        _database = database;
        _userService = userService;
        _serviceProvider = serviceProvider;
    }

    [RelayCommand]
    private async Task Send()
    {
        if (IsBusy || string.IsNullOrEmpty(Message)) return;

        IsBusy = true;
        try
        {
            var chatDto = await _apiClient
                .SendTextAsync(new PrivateChatMessageRequest(
                    CurrentSelectedSession.User.Id,
                    Message,
                    ChatMessageSendType.Personal));
            chatDto.IsSelfSend = true;
            var message = chatDto.ToChatMessage();
            message.FromAvatar = (await _userService.GetUserInfoByIdAsync(message.FromId)).Avatar;
            message.ToAvatar = (await _userService.GetUserInfoByIdAsync(message.ToId)).Avatar;
            CurrentSelectedSession.AddMessage(message);
            await _database.SaveChatMessageAsnyc(message);
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
    private Task Search(string searchText)
    {
        Toast.Make("Search:" + searchText).Show();
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task JoinDetail(Session item)
    {
        if (DeviceInfo.Idiom == DeviceIdiom.Desktop)
        {
            CurrentSelectedSession = item;

            if (!_viewCache.TryGetValue(item, out var view))
            {
                var viewModel = _serviceProvider.GetRequiredService<ChatMessageViewModel>();
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
        if (DeviceInfo.Idiom == DeviceIdiom.Desktop)
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