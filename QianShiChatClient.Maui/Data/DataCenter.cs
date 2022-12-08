namespace QianShiChatClient.Maui.Data;

public sealed partial class DataCenter : ObservableObject
{
    readonly IDispatcher _dispatcher;
    readonly IApiClient _apiClient;
    readonly ChatDatabase _database;
    readonly ChatHub _chatHub;

    [ObservableProperty]
    bool _isConnected;

    public SortableObservableCollection<Session> Sessions { get; }

    public DataCenter(IApiClient apiClient, ChatDatabase database, ChatHub chatHub, IDispatcher dispatcher)
    {
        _apiClient = apiClient;
        _database = database;
        _chatHub = chatHub;
        _dispatcher = dispatcher;
        Sessions = new SortableObservableCollection<Session>();
        Sessions.Descending = true;
        Sessions.SortingSelector = (t) => t.LastMessageTime;

        _chatHub.PrivateChat += ChatHubPrivateChat;
        _chatHub.IsConnectedChanged += (val) => IsConnected = val;
        _isConnected = _chatHub.IsConnected;

        _ = GetSessionsAsync();
        _ = GetUnreadMessageAsync();
    }

    void ChatHubPrivateChat(ChatMessageDto obj)
    {
        _dispatcher.Dispatch(async () =>
        {
            var session = Sessions.FirstOrDefault(x => x.User.Id == obj.FromId);
            var message = obj.ToChatMessage();
            if (session != null)
            {
                session.AddMessage(message);
            }
            else
            {
                // query new user.
                var user = await _database.GetUserByIdAsync(obj.FromId);
                if (user == null)
                {
                    // TODO: call api query user.
                }
                session = new Session(user, new List<ChatMessage>() { message });
                Sessions.Add(session);
            }
            await _database.SaveChatMessageAsnyc(message);
            await _database.SaveSessionAsync(session.ToSessionModel());
        });
    }

    async Task GetSessionsAsync()
    {
        var sessionModels = await _database.GetAllSessionAsync();

        foreach (var model in sessionModels)
        {
            if (model.Type == ChatMessageSendType.Personal)
            {
                var user = await _database.GetUserByIdAsync(model.ToId);
                if(user != null)
                {
                    var messages = await _database.GetChatMessageAsync(user.Id);
                    var session = new Session(user, messages);
                    _dispatcher.Dispatch(() => Sessions.Add(session));
                }
            }
        }
    }

    async Task GetUnreadMessageAsync()
    {
        var users = await _apiClient.GetUnreadMessageFriendsAsync();
        foreach (var userDto in users)
        {
            var messages = userDto.Messages.ToChatMessages();
            var user = userDto.ToUserInfo();
            user.Avatar = _apiClient.FormatFile(user.Avatar);
            foreach (var message in messages)
            {
                message.IsSelfSend = message.FromId == App.Current.User.Id;
            }

            var session = Sessions.FirstOrDefault(x => x.User.Id == userDto.Id);
            _dispatcher.Dispatch(() =>
            {
                if (session != null)
                {

                    session.User.Avatar = user.Avatar;
                    session.User.NickName = user.NickName;
                    session.AddMessages(messages);
                }
                else
                {
                    session = new Session(user, messages);
                    Sessions.Add(session);
                }
            });

            await _database.SaveUserAsync(user);
            if (messages.Count() > 0)
            {
                await _database.SaveChatMessagesAsnyc(messages);
            }
            await _database.SaveSessionAsync(session.ToSessionModel());
        }
    }
}
