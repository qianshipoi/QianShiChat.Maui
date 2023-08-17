namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class MessageDetailViewModel : ViewModelBase, IQueryAttributable
{
    private readonly DataCenter _dataCenter;
    private readonly IUserService _userService;
    private readonly IChatMessageRepository _chatMessageRepository;

    public int SessionId { get; private set; }

    [ObservableProperty]
    private SessionModel _session;

    [ObservableProperty]
    private ChatMessageModel _toMessage;

    [ObservableProperty]
    private bool _scrollAnimated;

    [ObservableProperty]
    private string _message;

    public MessageDetailViewModel(
        DataCenter dataCenter,
        IUserService userService,
        IChatMessageRepository chatMessageRepository)
    {
        _dataCenter = dataCenter;
        _userService = userService;
        _chatMessageRepository = chatMessageRepository;
    }

    [RelayCommand]
    private async Task Send()
    {
        if (IsBusy || string.IsNullOrEmpty(Message)) return;

        IsBusy = true;
        try
        {
            var message = await _dataCenter.SendTextAsync(User, Session, Message);

            ScrollAnimated = true;
            ToMessage = message;
            Message = string.Empty;
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
            var user = await _userService.GetUserInfoByIdAsync(SessionId);
            if (user != null)
            {
                var message = await _chatMessageRepository.GetChatMessageAsync(user.Id);
                Session = new SessionModel(user, message.Select(x=>x.ToChatMessageModel()));
            }
        }
    }
}