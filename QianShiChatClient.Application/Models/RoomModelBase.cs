namespace QianShiChatClient.Application.Models;

public abstract partial class RoomModelBase : ObservableObject
{
    protected readonly IUserService UserService;
    protected readonly IRoomRemoteService RoomRemoteService;

    private readonly ObservableCollection<MessageModel> _messages;
    public ReadOnlyObservableCollection<MessageModel> Messages { get; }
    public string Id { get; init; }
    public int ToId { get; }
    public ChatMessageSendType SendType { get; }
    [ObservableProperty]
    private string _displayName = string.Empty;
    [ObservableProperty]
    private int _unreadCount;
    [ObservableProperty]
    private long _lastMessageTime;
    [ObservableProperty]
    private string? _lastMessageContent;
    [ObservableProperty]
    private string? avatarPath;
    [ObservableProperty]
    private bool _hasMore = true;
    private int _page = 1;
    private int _size = 10;

    public RoomModelBase(string id, int toId, ChatMessageSendType sendType)
    {
        Id = id;
        UserService = ServiceHelper.GetReqiredService<IUserService>();
        RoomRemoteService = ServiceHelper.GetReqiredService<IRoomRemoteService>();
        _messages = new ObservableCollection<MessageModel>();
        Messages = new ReadOnlyObservableCollection<MessageModel>(_messages);
        ToId = toId;
        SendType = sendType;
    }

    public virtual void AddMessage(MessageModel message)
    {
        if (Messages.Any(x => x.Id == message.Id)) return;
        _messages.Add(message);
        UpdateLastInfo();
    }

    private void UpdateLastInfo()
    {
        if (!Messages.Any()) return;
        var lastMessage = Messages.Last();
        UnreadCount = lastMessage.IsSelfSend ? 0 : UnreadCount + 1;
        LastMessageTime = lastMessage.CreateTime;
        LastMessageContent = lastMessage.Content;
    }

    public virtual void AddMessages(IEnumerable<MessageModel> messages)
    {
        if (!messages.Any()) return;
        foreach (var message in messages)
        {
            if (Messages.Any(x => x.Id == message.Id))
            {
                break;
            }
            _messages.Add(message);
        }
        UpdateLastInfo();
    }

    public async Task GetHistoryAsync(CancellationToken cancellationToken = default)
    {
        if (!HasMore) return;
        var result = await RoomRemoteService.GetHistoryAsync(Id, _page, _size, cancellationToken);
        _page++;
        var messages = new List<MessageModel>();
        foreach (var item in result.Items)
        {
            messages.Add(await MessageDtoToMessageModelAsync(item));
        }
        AddMessages(messages);
        HasMore = result.Total > Messages.Count;
    }

    private async Task<MessageModel> MessageDtoToMessageModelAsync(ChatMessageDto dto)
    {
        var fromUser = UserService.CurrentUser()!;
        var isSelfSend = true;

        if (dto.FromId != fromUser.Id)
        {
            fromUser = await UserService.GetUserInfoByIdAsync(fromUser.Id);
            isSelfSend = false;
        }
        return new MessageModel(fromUser, isSelfSend, dto.Attachments.Select(x => new AttachmentModel
        {
            Id = x.Id,
            ContentType = x.ContentType,
            Hash = x.Hash,
            Name = x.Name,
            PreviewPath = x.PreviewPath,
            RawPath = x.RawPath,
            Size = x.Size
        }))
        {
            Content = dto.Content,
            CreateTime = dto.CreateTime,
            FromId = dto.FromId,
            Id = dto.Id,
            MessageType = dto.MessageType,
            SendType = dto.SendType,
            Status = MessageStatus.Successful,
            ToId = dto.ToId,
        };
    }
}
