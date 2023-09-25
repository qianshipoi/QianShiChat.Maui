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
    private readonly IRoomRemoteService _roomRemoteService;
    private IGlobalDispatcherTimer? _timer;

    [ObservableProperty]
    private bool _isConnected;

    public ObservableCollection<SessionModel> Sessions { get; }
    public ObservableCollection<ApplyPending> Pendings { get; }
    public ObservableCollection<UserInfoModel> Friends { get; }
    public ObservableCollection<RoomModelBase> Rooms { get; }

    public DataCenter(
        IApiClient apiClient,
        ChatHub chatHub,
        IGlobalDispatcher dispatcher,
        ILogger<DataCenter> logger,
        IUserService userService,
        IUserInfoRepository userInfoRepository,
        ISessionRepository sessionRepository,
        IChatMessageRepository chatMessageRepository,
        IRoomRemoteService roomRemoteService)
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
        Rooms = new();
        Sessions.CollectionChanged += Sessions_CollectionChanged;
        _chatHub.PrivateChat += ChatHubPrivateChat;
        _chatHub.IsConnectedChanged += ChatHub_IsConnectedChanged;
        IsConnected = _chatHub.IsConnected;
        _userService = userService;
        _roomRemoteService = roomRemoteService;

        _ = GetSessionsAsync();
        _ = GetApplyPendingsAsync();
        _ = GetAllFriendAsync();
    }

    private void ChatHub_IsConnectedChanged(bool val)
    {
        IsConnected = val;
        if (val && !Rooms.Any())
        {
            _ = GetRooms();
        }
    }

    private async Task GetRooms()
    {
        await foreach (var roomDto in _roomRemoteService.GetRoomsAsync())
        {
            if (roomDto.Type == ChatMessageSendType.Personal)
            {
                var user = await _userService.GetUserInfoByIdAsync(roomDto.ToId);
                Rooms.Add(new UserRoomModel(roomDto.Id, user));
            }
            else if (roomDto.Type == ChatMessageSendType.Group)
            {
                var group = await _roomRemoteService.GetGroupByIdAsync(roomDto.ToId);
                if (group is null)
                {
                    return;
                }
                var groupModel = new GroupModel
                {
                    Avatar = group.Avatar,
                    CreateTime = group.CreateTime,
                    Id = group.Id,
                    Name = group.Name,
                    TotalUser = group.TotalUser,
                    UserId = group.UserId,
                };
                Rooms.Add(new GroupRoomModel(roomDto.Id, groupModel));
            }
        }
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
        var session = Rooms.FirstOrDefault(x => x.Id == obj.RoomId);
        var user = await _userService.GetUserInfoByIdAsync(obj.FromId);
        if (session is null)
        {
            var room = await _roomRemoteService.GetRoomAsync(obj.ToId, obj.SendType);
            if (obj.SendType == ChatMessageSendType.Personal)
            {
                session = new UserRoomModel(obj.RoomId, user);
            }
            else if (obj.SendType == ChatMessageSendType.Group)
            {
                var group = await _roomRemoteService.GetGroupByIdAsync(obj.FromId);
                if (group == null) return;
                var groupModel = new GroupModel
                {
                    Avatar = group.Avatar,
                    CreateTime = group.CreateTime,
                    Id = group.Id,
                    Name = group.Name,
                    TotalUser = group.TotalUser,
                    UserId = group.UserId,
                };
                session = new GroupRoomModel(obj.RoomId, groupModel);
            }
            else
            {
                throw new NotSupportedException("未实现房间类型");
            }
        }
        var message = obj.ToChatMessage();
        if (message is null)
        {
            return;
        }
        var currentUser = _userService.CurrentUser();

        var messageModel = new MessageModel(user, currentUser.Id == user.Id, obj.Attachments.Select(x => new AttachmentModel
        {
            ContentType = x.ContentType,
            Hash = x.Hash,
            Id = x.Id,
            Name = x.Name,
            PreviewPath = x.PreviewPath,
            RawPath = x.RawPath,
            Size = x.Size,
        }))
        {
            Content = obj.Content,
            CreateTime = obj.CreateTime,
            FromId = user.Id,
            Id = user.Id,
            MessageType = message.MessageType,
            SendType = message.SendType,
            Status = message.Status,
            ToId = user.Id,
        };

        session.AddMessage(messageModel);
        UpdateRooms(session);
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
                        session = new SessionModel(_userService, user, messages.Select(x => x.ToChatMessageModel()));
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
            session = new SessionModel(_userService, user, messages);
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

    private MessageModel CreateSendMessage(
        UserInfoModel user,
        int toId,
        string content,
        ChatMessageType messageType = ChatMessageType.Text,
        ChatMessageSendType sendType = ChatMessageSendType.Personal)
    {
        return new MessageModel(user, true)
        {
            MessageType = messageType,
            SendType = sendType,
            Status = MessageStatus.Sending,
            CreateTime = Timestamp.Now,
            FromId = user.Id,
            ToId = toId,
            IsSelfSend = true,
            Content = content,
            Id = Timestamp.Now
        };
    }

    public MessageModel SendText(RoomModelBase room, string text)
    {
        var currentUser = _userService.CurrentUser();
        var message = CreateSendMessage(currentUser, room.ToId, text, ChatMessageType.Text, room.SendType);

        _ = Task.Run(async () => {
            try
            {
                var chatDto = await _apiClient
                      .SendTextAsync(new PrivateChatMessageRequest(
                          room.ToId,
                          text,
                          ChatMessageSendType.Personal));
                message.Id = chatDto!.Id;
                _dispatcher.Dispatch(() => {
                    message.Status = MessageStatus.Successful;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "send text message error.");
            }
        });
        room.AddMessage(message);
        UpdateRooms(room);
        return message;
    }

    private void UpdateRooms(RoomModelBase room)
    {
        var index = Rooms.IndexOf(room);

        if (index != -1)
        {
            Rooms.Move(index, 0);
        }
        else if (index != 0)
        {
            Rooms.Insert(0, room);
        }
    }

    public MessageModel SendFile(RoomModelBase room, int attachmentId)
    {
        var currentUser = _userService.CurrentUser();
        var message = CreateSendMessage(currentUser, room.ToId, "", ChatMessageType.OtherFile, room.SendType);

        _ = Task.Run(async () => {
            try
            {
                var chatDto = await _apiClient.SendAttachmentAsync(new AttachmentMessageRequest
                (
                    room.ToId,
                    attachmentId,
                    room.SendType
                ));

                message.Id = chatDto!.Id;
                _dispatcher.Dispatch(() => {
                    message.Status = MessageStatus.Successful;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "send text message error.");
            }
        });

        room.AddMessage(message);

        UpdateRooms(room);

        return message;
    }
}