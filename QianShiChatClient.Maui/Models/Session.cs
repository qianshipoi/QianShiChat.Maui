namespace QianShiChatClient.Maui.Models;

public partial class Session : ObservableObject
{
    [ObservableProperty]
    private int _unreadCount;

    [ObservableProperty]
    private long _lastMessageTime;

    [ObservableProperty]
    private string _lastMessageContent;

    public UserInfo User { get; }

    public ObservableCollection<ChatMessage> Messages { get; }

    public Session(UserInfo user, IEnumerable<ChatMessage> messages)
    {
        User = user;
        Messages = new ObservableCollection<ChatMessage>();
        _ = AddMessagesAsync(messages);
    }

    public async Task AddMessagesAsync(IEnumerable<ChatMessage> messages, CancellationToken cancellationToken = default)
    {
        var ids = messages.Select(x => x.FromId).Concat(messages.Select(x => x.ToId)).ToHashSet();
        var dic = new Dictionary<int, UserInfo>();
        var userService = ServiceHelper.GetService<IUserService>();

        foreach (var id in ids)
        {
            var userInfo = await userService.GetUserInfoByIdAsync(id, cancellationToken);
            dic.Add(id, userInfo);
        }

        var orderMessages = messages.OrderBy(x => x.CreateTime);
        foreach (var message in orderMessages)
        {
            message.FromAvatar = dic[message.FromId]?.Avatar;
            message.ToAvatar = dic[message.ToId]?.Avatar;
            await AddMessageAsync(message, cancellationToken);
        }
    }

    public async Task AddMessageAsync(ChatMessage message, CancellationToken cancellationToken = default)
    {
        if (Messages.Any(x => x.Id == message.Id))
        {
            return;
        }

        var userService = ServiceHelper.GetService<IUserService>();
        if (string.IsNullOrWhiteSpace(message.FromAvatar))
        {
            message.FromAvatar = (await userService.GetUserInfoByIdAsync(message.FromId, cancellationToken)).Avatar;

        }
        if (string.IsNullOrWhiteSpace(message.ToAvatar))
        {
            message.ToAvatar = (await userService.GetUserInfoByIdAsync(message.ToId, cancellationToken)).Avatar;
        }

        Messages.Add(message);
        UnreadCount = message.IsSelfSend ? 0 : UnreadCount + 1;
        LastMessageTime = message.CreateTime;
        LastMessageContent = message.Content;
    }
}