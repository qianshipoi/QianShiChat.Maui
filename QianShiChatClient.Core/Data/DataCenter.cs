using System.Collections.Specialized;

namespace QianShiChatClient.Core.Data;

public sealed partial class DataCenter : ObservableObject
{
    private readonly IDispatcher _dispatcher;
    private readonly IApiClient _apiClient;
    private readonly ChatDatabase _database;
    private readonly ChatHub _chatHub;
    private readonly ILogger<DataCenter> _logger;
    private readonly IUserService _userService;
    private IDispatcherTimer _timer;

    [ObservableProperty]
    private bool _isConnected;

    public ObservableCollection<Session> Sessions { get; }

    public ObservableCollection<ApplyPending> Pendings { get; }

    public ObservableCollection<UserInfo> Friends { get; }

    public DataCenter(
        IApiClient apiClient,
        ChatDatabase database,
        ChatHub chatHub,
        IDispatcher dispatcher,
        ILogger<DataCenter> logger,
        IUserService userService)
    {
        _apiClient = apiClient;
        _database = database;
        _chatHub = chatHub;
        _dispatcher = dispatcher;
        _logger = logger;
        Friends = new ObservableCollection<UserInfo>();
        Pendings = new ObservableCollection<ApplyPending>();
        Sessions = new ObservableCollection<Session>();
        Sessions.CollectionChanged += Sessions_CollectionChanged;
        _chatHub.PrivateChat += ChatHubPrivateChat;
        _chatHub.IsConnectedChanged += (val) => IsConnected = val;
        IsConnected = _chatHub.IsConnected;
        _userService = userService;

        _ = GetSessionsAsync();
        _ = GetApplyPendingsAsync();
        _ = GetAllFriendAsync();
    }

    private void Sessions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        // todo:optimize - record order by last message time.
        ClearTimer();
        _timer = _dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromSeconds(3);
        _timer.IsRepeating = false;
        _timer.Tick += Timer_Tick;
        _timer.Start();
    }

    private void ClearTimer()
    {
        if (_timer is not null)
        {
            _timer.Tick -= Timer_Tick;
            _timer.Stop();
        }
    }

    private async void Timer_Tick(object sender, EventArgs e) => await SaveSessionsAsync();

    private async Task SaveSessionsAsync()
    {
        await _database.SaveSessionsAsync(Sessions.OrderByDescending(x => x.LastMessageTime).Select(x => x.ToSessionModel()));
        ClearTimer();
    }

    private async void ChatHubPrivateChat(ChatMessageDto obj)
    {
        var session = Sessions.FirstOrDefault(x => x.User.Id == obj.FromId);
        var message = obj.ToChatMessage();
        if (message is null)
        {
            var user = await _userService.GetUserInfoByIdAsync(obj.FromId);
            session = new Session(user, new List<ChatMessage>());
            _dispatcher.Dispatch(() => {
                Sessions.Add(session);
            });
        }
        await session.AddMessageAsync(message);
        await _database.SaveChatMessageAsnyc(message);
        await _database.SaveSessionAsync(session.ToSessionModel());
    }

    private async Task GetSessionsAsync()
    {
        var sessionModels = await _database.GetAllSessionAsync();

        foreach (var model in sessionModels)
        {
            if (model.Type == ChatMessageSendType.Personal)
            {
                var session = Sessions.FirstOrDefault(x => x.User.Id == model.ToId);
                if (session == null)
                {
                    var user = await _userService.GetUserInfoByIdAsync(model.ToId);
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

    private async Task<Session> AddSessionsAsync(UserInfo user, IEnumerable<ChatMessage> messages)
    {
        var session = Sessions.FirstOrDefault(x => x.User.Id == user.Id);
        if (session is null)
        {
            session = new Session(user, messages);
            _dispatcher.Dispatch(() => {
                Sessions.Add(session);
            });

        }
        session.User.Avatar = user.Avatar;
        session.User.NickName = user.NickName;
        await session.AddMessagesAsync(messages);
        return session;
    }

    public async Task GetUnreadMessageAsync()
    {
        var users = await _apiClient.GetUnreadMessageFriendsAsync();
        if (users is null) return;
        foreach (var userDto in users)
        {
            var messages = userDto.Messages.ToChatMessages();
            var user = userDto.ToUserInfo();
            foreach (var message in messages)
            {
                message.IsSelfSend = message.FromId == _userService.CurrentUser().Id;
            }

            var session = await AddSessionsAsync(user, messages);

            await _database.SaveUserAsync(user);
            if (messages.Any())
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
                Pendings.Add(apply);
            }
        }
    }

    public async Task GetAllFriendAsync()
    {
        var userdtos = await _apiClient.AllFriendAsync();
        if (userdtos is null) return;
        foreach (var userDto in userdtos)
        {
            var user = userDto.ToUserInfo();

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

    private ChatMessage CreateSendMessage(
        UserInfo user,
        int toId,
        string content,
        ChatMessageType messageType = ChatMessageType.Text)
    {
        return new ChatMessage
        {
            LocalId = Guid.NewGuid(),
            MessageType = messageType,
            SendType = ChatMessageSendType.Personal,
            Status = MessageStatus.Sending,
            CreateTime = Timestamp.Now,
            FromId = user.Id,
            ToId = toId,
            FromAvatar = user.Avatar,
            ToAvatar = user.Avatar,
            IsSelfSend = true,
            Content = content
        };
    }

    public async Task<ChatMessage> SendTextAsync(UserInfo user, Session session, string text)
    {
        var message = CreateSendMessage(user, session.User.Id, text);

        _ = Task.Run(async () => {
            try
            {
                var chatDto = await _apiClient
                      .SendTextAsync(new PrivateChatMessageRequest(
                          session.User.Id,
                          text,
                          ChatMessageSendType.Personal));
                message.Id = chatDto.Id;
                _dispatcher.Dispatch(() => {
                    message.Status = MessageStatus.Successful;
                });
                await _database.UpdateChatMessageAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "send text message error.");
            }
        });

        await session.AddMessageAsync(message);

        UpdateSessions(session);
        try
        {
            await _database.SaveChatMessageAsnyc(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "");
        }

        return message;
    }

    private void UpdateSessions(Session session)
    {
        var index = Sessions.IndexOf(session);

        if (index != -1)
        {
            Sessions.Move(index, 0);
        }
        else if (index != 0)
        {
            Sessions.Insert(0, session);
        }
    }

    private Task<ChatMessageDto> MockSendFileAsync(Action<double, double> uploadProgressValue)
    {
        var total = 25;
        return Task.Run(async () => {
            for (int i = 0; i < total; i++)
            {
                await Task.Delay(250);
                uploadProgressValue.Invoke(i + 1, total);
            }
            return new ChatMessageDto { Id = DateTime.Now.Ticks };
        });
    }

    public async Task<ChatMessage> SendFileAsync(UserInfo user, Session session, string filePath)
    {
        var message = CreateSendMessage(user, session.User.Id, filePath, ChatMessageType.OtherFile);

        _ = Task.Run(async () => {
            try
            {
                var chatDto = await MockSendFileAsync((loaded, total) => {
                    MainThread.BeginInvokeOnMainThread(() => {
                        message.UploadProgressValue = loaded / total;
                    });
                });

                //var chatDto = await _apiClient
                //      .SendFileAsync(
                //    session.User.Id,
                //    ChatMessageSendType.Personal,
                //    filePath,
                //    (loaded, total) => {
                //        MainThread.BeginInvokeOnMainThread(() => {
                //            message.UploadProgressValue = loaded / total;
                //        });
                //    });
                message.Id = chatDto.Id;
                _dispatcher.Dispatch(() => {
                    message.Status = MessageStatus.Successful;
                });
                await _database.UpdateChatMessageAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "send text message error.");
            }
        });

        await session.AddMessageAsync(message);

        UpdateSessions(session);
        try
        {
            await _database.SaveChatMessageAsnyc(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "");
        }

        return message;
    }
}