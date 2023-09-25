namespace QianShiChatClient.Application.Models;

public record LoginRequest(string Account, string Password);

public record PrivateChatMessageRequest(int ToId, string Message, ChatMessageSendType SendType);
public record AttachmentMessageRequest(int ToId, int AttachmentId, ChatMessageSendType SendType);

public record FriendApplyRequest(int UserId, string Remark);

public record FriendApplyPendingRequest(int Size, long? BeforeLastTime = null);

public class GlobalResult<T>
{
    /// <summary>
    /// 状态码
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// 数据
    /// </summary>
    public T Data { get; set; } = default!;

    /// <summary>
    /// 执行成功
    /// </summary>
    public bool Succeeded { get; set; }

    /// <summary>
    /// 错误信息
    /// </summary>
    public object? Errors { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public long Timestamp { get; set; }
}

public record UserDto(int Id, string Account, string NickName, string Avatar, long CreateTime);
public record GroupDto(int Id, int UserId, string Name, string Avatar, int TotalUser, long CreateTime);

public class QrAuthResponse
{
    public int Code { get; set; }

    public string Message { get; set; } = default!;
}

public class CheckQrAuthKeyResponse
{
    /// <summary>
    /// 800 二维码不存在或已过期 801 等待扫码 802 授权中 803 授权成功
    /// </summary>
    public int Code { get; set; }

    public string Message { get; set; } = string.Empty;
    public UserDto? User { get; set; }
    public string? AccessToken { get; set; }
}

public class CreateQrAuthKeyResponse
{
    public int Code { get; set; }

    public string Key { get; set; } = string.Empty;
}

public class CreateQrCodeRequest
{
    public string Key { get; set; } = string.Empty;

    public bool Qrimg { get; set; }
}

public class CreateQrCodeResponse
{
    public string Url { get; set; } = string.Empty;

    public string? Image { get; set; }
}

public class ApplyPendingDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int FriendId { get; set; }
    public long CreateTime { get; set; }
    public int Status { get; set; }
    public string? Remark { get; set; }
    public UserDto User { get; set; } = default!;
    public UserDto Friend { get; set; } = default!;
}

public class PagedList<T> where T : class
{
    public int Total { get; set; }
    public List<T> Items { get; set; } = default!;
}

public record UserWithMessageDto : UserDto
{
    public UserWithMessageDto(int Id, string Account, string NickName, string Avatar, long CreateTime, List<ChatMessageDto> Messages)
        : base(Id, Account, NickName, Avatar, CreateTime)
    {
        this.Messages = Messages;
    }

    public List<ChatMessageDto> Messages { get; init; }
}

public record ChatMessageDto
{
    public long Id { get; set; }
    public int FromId { get; set; }
    public int ToId { get; set; }
    public string RoomId { get; set; } = string.Empty;
    public ChatMessageSendType SendType { get; set; }
    public ChatMessageType MessageType { get; set; }
    public string Content { get; set; } = null!;
    public long CreateTime { get; set; }
    public bool IsSelfSend { get; set; }
    public UserDto? FromUser { get; set; }

    public List<AttachmentDto> Attachments { get; set; } = new List<AttachmentDto>();
}

public class AttachmentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string RawPath { get; set; } = string.Empty;
    public string? PreviewPath { get; set; }
    public string Hash { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long Size { get; set; }
}

public class RoomDto
{
    public string Id { get; set; } = string.Empty;
    public ChatMessageSendType Type { get; set; }
    public int FromId { get; set; }
    public int ToId { get; set; }
    public int UnreadCount { get; set; }
    public long LastMessageTime { get; set; }
    public object? LastMessageContent { get; set; }
    public UserDto? FromUser { get; set; }
    public object? ToObject { get; set; }
}

public record NotificationMessage(NotificationType Type, string Message);
