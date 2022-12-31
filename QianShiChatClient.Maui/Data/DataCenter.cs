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

    public ObservableCollection<ApplyPending> Pendings { get; }

    public ObservableCollection<UserInfo> Friends { get; }

    public DataCenter(IApiClient apiClient, ChatDatabase database, ChatHub chatHub, IDispatcher dispatcher)
    {
        _apiClient = apiClient;
        _database = database;
        _chatHub = chatHub;
        _dispatcher = dispatcher;
        Friends = new ObservableCollection<UserInfo>();
        Pendings = new ObservableCollection<ApplyPending>();
        Sessions = new SortableObservableCollection<Session>();
        Sessions.Descending = true;
        Sessions.SortingSelector = (t) => t.LastMessageTime;
        Sessions.CollectionChanged += Sessions_CollectionChanged;

        _chatHub.PrivateChat += ChatHubPrivateChat;
        _chatHub.IsConnectedChanged += (val) => IsConnected = val;
        IsConnected = _chatHub.IsConnected;

        _ = GetSessionsAsync();
        _ = GetApplyPendingsAsync();
        _ = GetAllFriendAsync();
    }

    async void Sessions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        foreach (var session in Sessions)
        {
            await _database.SaveSessionAsync(session.ToSessionModel());
        }
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
                var session = Sessions.FirstOrDefault(x => x.User.Id == model.ToId);
                if (session == null)
                {
                    var user = await _database.GetUserByIdAsync(model.ToId);
                    if (user != null)
                    {
                        var messages = await _database.GetChatMessageAsync(user.Id, 0, 20);
                        session = new Session(user, messages);
                        _dispatcher.Dispatch(() => Sessions.Add(session));
                    }
                }
            }
        }
    }

    Session AddSessions(UserInfo user, IEnumerable<ChatMessage> messages)
    {
        var session = Sessions.FirstOrDefault(x => x.User.Id == user.Id);
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
        return session;
    }

    public async Task GetUnreadMessageAsync()
    {
        var users = await _apiClient.GetUnreadMessageFriendsAsync();
        foreach (var userDto in users)
        {
            var messages = userDto.Messages.ToChatMessages();
            var user = userDto.ToUserInfo();
            //user.Avatar = _apiClient.FormatFile(user.Avatar);
            foreach (var message in messages)
            {
                message.IsSelfSend = message.FromId == App.Current.User.Id;
            }

            var session = AddSessions(user, messages);

            await _database.SaveUserAsync(user);
            if (messages.Count() > 0)
            {
                await _database.SaveChatMessagesAsnyc(messages);
            }
            await _database.SaveSessionAsync(session.ToSessionModel());
        }
    }

    public async Task GetApplyPendingsAsync()
    {
        var paged = await _apiClient.FriendApplyPendingAsync(new FriendApplyPendingRequest(20));
        if (paged != null && paged.Items.Count > 0)
        {
            foreach (var item in paged.Items)
            {
                var apply = item.ToApplyPending();
                //apply.User.Avatar = _apiClient.FormatFile(apply.User.Avatar);
                //apply.Friend.Avatar = _apiClient.FormatFile(apply.Friend.Avatar);
                Pendings.Add(apply);
            }
        }
    }

    public async Task GetAllFriendAsync()
    {
        var userdtos = await _apiClient.AllFriendAsync();
        foreach (var userDto in userdtos)
        {
            var user = userDto.ToUserInfo();
            //user.Avatar = _apiClient.FormatFile(userDto.Avatar);

            var friend = Friends.FirstOrDefault(x => x.Id == user.Id);

            if (friend == null)
            {
                Friends.Add(user);
            }
            else
            {
                friend.Avatar = user.Avatar;
                friend.NickName = user.NickName;
                friend.Content = user.Content;
            }
            await _database.SaveUserAsync(user);
        }
    }
}
