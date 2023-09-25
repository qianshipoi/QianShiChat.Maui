using QianShiChatClient.Application.IServices;

namespace QianShiChatClient.Application.Models;

public partial class MessageModel : ObservableObject
{
    [ObservableProperty]
    private long _id;
    [ObservableProperty]
    private int _fromId;
    [ObservableProperty]
    private int toId;
    [ObservableProperty]
    private ChatMessageSendType _sendType;
    [ObservableProperty]
    private ChatMessageType _messageType;
    [ObservableProperty]
    private string _content = string.Empty;
    [ObservableProperty]
    private long _createTime;
    [ObservableProperty]
    private MessageStatus _status;
    public UserInfoModel FromUser { get; init; }
    public bool IsSelfSend { get; init; }
    public ObservableCollection<AttachmentModel> Attachments;

    public MessageModel(UserInfoModel fromUser, bool isSelf, IEnumerable<AttachmentModel>? attachments = null)
    {
        FromUser = fromUser;
        Attachments = new ObservableCollection<AttachmentModel>(attachments ?? new List<AttachmentModel>());
        IsSelfSend = isSelf;
    }
}

public partial class AttachmentModel : ObservableObject
{
    [ObservableProperty]
    private long _id;
    [ObservableProperty]
    private string _name = string.Empty;
    [ObservableProperty]
    private string _rawPath = string.Empty;
    [ObservableProperty]
    private string? _previewPath = string.Empty;
    [ObservableProperty]
    private string _hash = string.Empty;
    [ObservableProperty]
    private string _contentType = string.Empty;
    [ObservableProperty]
    private long _size;
    [ObservableProperty]
    private double _progress;
}

public abstract partial class RoomModelBase : ObservableObject
{
    protected readonly IUserService UserService;
    protected readonly IRoomRemoteService RoomRemoteService;

    private readonly ObservableCollection<MessageModel> _messages;
    public ReadOnlyObservableCollection<MessageModel> Messages { get; }
    public string Id { get; init; }
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

    public RoomModelBase(string id)
    {
        UserService = ServiceHelper.GetReqiredService<IUserService>();
        RoomRemoteService = ServiceHelper.GetReqiredService<IRoomRemoteService>();
        _messages = new ObservableCollection<MessageModel>();
        Messages = new ReadOnlyObservableCollection<MessageModel>(_messages);
        Id = id;
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

public partial class GroupRoomModel : RoomModelBase
{
    public GroupModel Group { get; init; }
    public GroupRoomModel(string id, GroupModel group) : base(id)
    {
        Group = group;
        AvatarPath = group.Avatar;
    }
}

public partial class UserRoomModel : RoomModelBase
{
    public UserInfoModel User { get; init; }
    public UserRoomModel(string id, UserInfoModel user) : base(id)
    {
        User = user;
        AvatarPath = user.Avatar;
    }
}

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
        _messages.Add(message);

        UnreadCount = message.IsSelfSend ? 0 : UnreadCount + 1;
        LastMessageTime = message.CreateTime;
        LastMessageContent = message.Content;
    }
}
