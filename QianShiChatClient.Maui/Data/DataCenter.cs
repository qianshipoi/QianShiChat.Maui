﻿namespace QianShiChatClient.Maui.Data;

public sealed partial class DataCenter : ObservableObject
{
    private readonly IDispatcher _dispatcher;
    private readonly IApiClient _apiClient;
    private readonly ChatDatabase _database;
    private readonly ChatHub _chatHub;
    private readonly ILogger<DataCenter> _logger;

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
        ILogger<DataCenter> logger)
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

        _ = GetSessionsAsync();
        _ = GetApplyPendingsAsync();
        _ = GetAllFriendAsync();
    }

    private void Sessions_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        // todo: save sorted sessions to database.
    }

    private async void ChatHubPrivateChat(ChatMessageDto obj)
    {
        var session = Sessions.FirstOrDefault(x => x.User.Id == obj.FromId);
        var message = obj.ToChatMessage();
        if (message is null)
        {
            var user = await _database.GetUserByIdAsync(obj.FromId);
            // todo: if user is null, need query user info.

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

    private async Task<Session> AddSessionsAsync(UserInfo user, IEnumerable<ChatMessage> messages)
    {
        var session = Sessions.FirstOrDefault(x => x.User.Id == user.Id);
        if (session != null)
        {
            session.User.Avatar = user.Avatar;
            session.User.NickName = user.NickName;
            await session.AddMessagesAsync(messages);
        }
        else
        {
            session = new Session(user, messages);
            _dispatcher.Dispatch(() => {
                Sessions.Add(session);
            });
        }
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
                message.IsSelfSend = message.FromId == App.Current.User.Id;
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

    public async Task<ChatMessage> SendTextAsync(UserInfo user, Session session, string text)
    {
        var message = new ChatMessage();
        message.LocalId = Guid.NewGuid();
        message.MessageType = ChatMessageType.Text;
        message.SendType = ChatMessageSendType.Personal;
        message.Status = MessageStatus.Sending;
        message.CreateTime = Timestamp.Now;
        message.Content = text;
        message.FromId = user.Id;
        message.ToId = session.User.Id;
        message.FromAvatar = user.Avatar;
        message.ToAvatar = user.Avatar;
        message.IsSelfSend = true;

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

        var index = Sessions.IndexOf(session);

        if (index != -1)
        {
            Sessions.Move(index, 0);
        }
        else if (index != 0)
        {
            Sessions.Insert(0, session);
        }

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