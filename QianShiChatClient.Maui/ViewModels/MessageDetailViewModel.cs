namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class MessageDetailViewModel : ViewModelBase, IQueryAttributable
{
    private readonly IApiClient _apiClient;
    private readonly ChatDatabase _database;
    private readonly DataCenter _dataCenter;
    private readonly IUserService _userService;

    private bool _isNewSession;

    public int SessionId { get; private set; }

    [ObservableProperty]
    private Session _session;

    [ObservableProperty]
    private ChatMessage _toMessage;

    [ObservableProperty]
    private bool _scrollAnimated;

    [ObservableProperty]
    private string _message;

    public MessageDetailViewModel(
        IStringLocalizer<MyStrings> stringLocalizer,
        INavigationService navigationService,
        IApiClient apiClient,
        ChatDatabase database,
        DataCenter dataCenter,
        IUserService userService)
        : base(navigationService, stringLocalizer)
    {
        _apiClient = apiClient;
        _database = database;
        _dataCenter = dataCenter;
        _userService = userService;
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
                    Session.User.Id,
                    Message,
                    ChatMessageSendType.Personal));
            chatDto.IsSelfSend = true;
            var message = chatDto.ToChatMessage();
            Session.AddMessage(message);
            await _database.SaveChatMessageAsnyc(message);
            ScrollAnimated = true;
            ToMessage = message;
            Message = string.Empty;
            if (_isNewSession)
            {
                _dataCenter.Sessions.Add(Session);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        SessionId = (int)query[nameof(SessionId)];

        var session = _dataCenter.Sessions.FirstOrDefault(x => x.User.Id == SessionId);
        if (session != null)
        {
            Session = session;
        }
        else
        {
            var user = await _database.GetUserByIdAsync(SessionId);
            if (user != null)
            {
                var message = await _database.GetChatMessageAsync(user.Id);
                Session = new Session(user, message, _userService);
                _isNewSession = true;
            }
        }
    }
}