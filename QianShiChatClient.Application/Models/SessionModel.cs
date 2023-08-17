namespace QianShiChatClient.Application.Models;

public partial class SessionModel : ObservableObject
{
    private readonly ObservableCollection<ChatMessageModel> _messages;
    private readonly IUserService _userService;

    public int Id { get; }

    [ObservableProperty]
    private int _unreadCount;

    [ObservableProperty]
    private long _lastMessageTime;

    [ObservableProperty]
    private string? _lastMessageContent;

    public UserInfoModel User { get; }

    public ReadOnlyObservableCollection<ChatMessageModel> Messages { get; }

    public SessionModel(IUserService userService, UserInfoModel user, IEnumerable<ChatMessageModel> messages, int id = 0)
    {
        _userService = userService;
        User = user;
        Id = id;
        _messages = new();
        Messages = new ReadOnlyObservableCollection<ChatMessageModel>(_messages);
        _ = AddMessagesAsync(messages);
        LastMessageTime = Timestamp.Now;
    }

    public async Task AddMessagesAsync(IEnumerable<ChatMessageModel> messages, CancellationToken cancellationToken = default)
    {
        var ids = messages.Select(x => x.FromId).Concat(messages.Select(x => x.ToId)).ToHashSet();
        var dic = new Dictionary<int, UserInfoModel>();

        foreach (var id in ids)
        {
            var userInfo = await _userService.GetUserInfoByIdAsync(id, cancellationToken);
            dic.Add(id, userInfo);
        }

        var orderMessages = messages.OrderBy(x => x.CreateTime);
        foreach (var message in orderMessages)
        {
            if (dic.TryGetValue(message.FromId, out var fromUser))
            {
                message.FromAvatar = fromUser.Avatar;
            }
            if (dic.TryGetValue(message.ToId, out var toUser))
            {
                message.ToAvatar = toUser.Avatar;
            }

            await AddMessageAsync(message, cancellationToken);
        }
    }

    public async Task AddMessageAsync(ChatMessageModel message, CancellationToken cancellationToken = default)
    {
        if (Messages.Any(x => x.LocalId == message.LocalId))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(message.FromAvatar))
        {
            message.FromAvatar = (await _userService.GetUserInfoByIdAsync(message.FromId, cancellationToken)).Avatar;

        }
        if (string.IsNullOrWhiteSpace(message.ToAvatar))
        {
            message.ToAvatar = (await _userService.GetUserInfoByIdAsync(message.ToId, cancellationToken)).Avatar;
        }

        _messages.Add(message);

        UnreadCount = message.IsSelfSend ? 0 : UnreadCount + 1;
        LastMessageTime = message.CreateTime;
        LastMessageContent = message.Content;
    }
}
