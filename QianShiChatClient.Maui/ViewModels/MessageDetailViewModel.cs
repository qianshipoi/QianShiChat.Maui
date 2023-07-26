namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class MessageDetailViewModel : ViewModelBase, IQueryAttributable
{
    private readonly ChatDatabase _database;
    private readonly DataCenter _dataCenter;

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
        ChatDatabase database,
        DataCenter dataCenter)
    {
        _database = database;
        _dataCenter = dataCenter;
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
            var user = await _database.GetUserByIdAsync(SessionId);
            if (user != null)
            {
                var message = await _database.GetChatMessageAsync(user.Id);
                Session = new Session(user, message);
            }
        }
    }
}