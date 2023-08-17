namespace QianShiChatClient.Application.Data;

public sealed partial class DataCenter : ObservableObject
{
    private readonly IGlobalDispatcher _dispatcher;
    private readonly IApiClient _apiClient;
    private readonly ChatHub _chatHub;
    private readonly ILogger<DataCenter> _logger;
    private readonly IUserService _userService;
    private readonly IUserInfoRepository _userInfoRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IChatMessageRepository _chatMessageRepository;
    private IGlobalDispatcherTimer? _timer;

    [ObservableProperty]
    private bool _isConnected;

    public ObservableCollection<SessionModel> Sessions { get; }

    public ObservableCollection<ApplyPending> Pendings { get; }

    public ObservableCollection<UserInfoModel> Friends { get; }

    public DataCenter(
        IApiClient apiClient,
        ChatHub chatHub,
        IGlobalDispatcher dispatcher,
        ILogger<DataCenter> logger,
        IUserService userService,
        IUserInfoRepository userInfoRepository,
        ISessionRepository sessionRepository,
        IChatMessageRepository chatMessageRepository)
    {
        _apiClient = apiClient;
        _chatHub = chatHub;
        _dispatcher = dispatcher;
        _logger = logger;
        _userInfoRepository = userInfoRepository;
        _sessionRepository = sessionRepository;
        _chatMessageRepository = chatMessageRepository;
        Friends = new();
        Pendings = new();
        Sessions = new();
        Sessions.CollectionChanged += Sessions_CollectionChanged;
        _chatHub.PrivateChat += ChatHubPrivateChat;
        _chatHub.IsConnectedChanged += (val) => IsConnected = val;
        IsConnected = _chatHub.IsConnected;
        _userService = userService;

        _ = GetSessionsAsync();
        _ = GetApplyPendingsAsync();
        _ = GetAllFriendAsync();
    }

    private void Sessions_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
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

    private async void Timer_Tick(object? sender, EventArgs e) => await SaveSessionsAsync();

    private async Task SaveSessionsAsync()
    {
        await _sessionRepository.SaveSessionAsync(Sessions.OrderByDescending(x => x.LastMessageTime).Select(x => x.ToSession()));
        ClearTimer();
    }

    private async void ChatHubPrivateChat(ChatMessageDto obj)
    {
        var session = Sessions.FirstOrDefault(x => x.User.Id == obj.FromId);
        if (session is null)
        {
            var user = await _userService.GetUserInfoByIdAsync(obj.FromId);
            session = new SessionModel(user, new List<ChatMessageModel>());
        }
        var message = obj.ToChatMessage();
        if (message is null)
        {
            return;
        }
        await session.AddMessageAsync(message.ToChatMessageModel());
        UpdateSessions(session);
        await _chatMessageRepository.SaveChatMessageAsnyc(message);
        await _sessionRepository.SaveSessionAsync(session.ToSession());
    }

    private async Task GetSessionsAsync()
    {
        var sessionModels = await _sessionRepository.GetAllSessionAsync();

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
                        var messages = await _chatMessageRepository.GetChatMessageAsync(user.Id, 0, 20);
                        session = new SessionModel(user, messages.Select(x => x.ToChatMessageModel()));
                        _dispatcher.Dispatch(() => Sessions.Add(session));
                    }
                }
            }
        }
    }

    private async Task<SessionModel> AddSessionsAsync(UserInfoModel user, IEnumerable<ChatMessageModel> messages)
    {
        var session = Sessions.FirstOrDefault(x => x.User.Id == user.Id);
        if (session is null)
        {
            session = new SessionModel(user, messages);
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
            var user = userDto.ToUserInfoModel();
            foreach (var message in messages)
            {
                message.IsSelfSend = message.FromId == _userService.CurrentUser().Id;
            }

            var session = await AddSessionsAsync(user, messages.Select(x => x.ToChatMessageModel()));

            await _userInfoRepository.SaveUserAsync(user.ToUserInfo());
            if (messages.Any())
            {
                await _chatMessageRepository.SaveChatMessagesAsnyc(messages);
            }
            await _sessionRepository.SaveSessionAsync(session.ToSession());
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
            var user = userDto.ToUserInfoModel();

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
            await _userInfoRepository.SaveUserAsync(user.ToUserInfo());
        }
    }

    private ChatMessageModel CreateSendMessage(
        UserInfoModel user,
        int toId,
        string content,
        ChatMessageType messageType = ChatMessageType.Text)
    {
        return new ChatMessageModel
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

    public async Task<ChatMessageModel> SendTextAsync(UserInfoModel user, SessionModel session, string text)
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
                message.Id = chatDto!.Id;
                _dispatcher.Dispatch(() => {
                    message.Status = MessageStatus.Successful;
                });

                await _chatMessageRepository.UpdateChatMessageAsync(message.ToChatMessage());
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
            await _chatMessageRepository.SaveChatMessageAsnyc(message.ToChatMessage());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "");
        }

        return message;
    }

    private void UpdateSessions(SessionModel session)
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

    public async Task<ChatMessageModel> SendFileAsync(UserInfoModel user, SessionModel session, string filePath)
    {
        var message = CreateSendMessage(user, session.User.Id, filePath, ChatMessageType.OtherFile);

        _ = Task.Run(async () => {
            try
            {
                //var chatDto = await MockSendFileAsync((loaded, total) => {
                //    MainThread.BeginInvokeOnMainThread(() => {
                //        message.UploadProgressValue = loaded / total;
                //    });
                //});
                var chatDto = await _apiClient
                      .SendFileAsync(
                    session.User.Id,
                    ChatMessageSendType.Personal,
                    filePath,
                    (loaded, total) => {
                        _dispatcher.Dispatch(() => {
                            message.UploadProgressValue = loaded / total;
                        });
                    });
                message.Id = chatDto!.Id;
                _dispatcher.Dispatch(() => {
                    message.Status = MessageStatus.Successful;
                });
                await _chatMessageRepository.UpdateChatMessageAsync(message.ToChatMessage());
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
            await _chatMessageRepository.SaveChatMessageAsnyc(message.ToChatMessage());
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "");
        }

        return message;
    }
}