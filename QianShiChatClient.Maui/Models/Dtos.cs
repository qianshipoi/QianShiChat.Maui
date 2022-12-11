namespace QianShiChatClient.Maui.Models;

public record LoginReqiest(string Account, string Password);

public record PrivateChatMessageRequest(int ToId, string Message, ChatMessageSendType SendType);

public record FriendApplyRequest(int UserId, string Remark);

public record FriendApplyPendingRequest(int Size, long? BeforeLastTime = null);

public record UserDto(int Id, string Account, string NickName, string Avatar, long CreateTime);

public class QrAuthResponse
{
    /// <summary>
    /// 800 二维码不存在或已过期 801 等待扫码 802 授权中 803 授权成功
    /// </summary>
    public int Code { get; set; }

    public string Message { get; set; }
}

public class ApplyPendingDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int FriendId { get; set; }
    public long CreateTime { get; set; }
    public int Status { get; set; }
    public string Remark { get; set; }
    public UserDto User { get; set; }
    public UserDto Friend { get; set; }
}

public class PagedList<T> where T : class
{
    public int Total { get; set; }
    public List<T> Items { get; set; }
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

    public ChatMessageSendType SendType { get; set; }

    public ChatMessageType MessageType { get; set; }

    public string Content { get; set; } = null!;

    public long CreateTime { get; set; }

    public bool IsSelfSend { get; set; }

    public UserDto FromUser { get; set; }
}

public record NotificationMessage(NotificationType Type, string Message);

public enum NotificationType
{
    /// <summary>
    /// 好友上线
    /// </summary>
    FriendOnline,

    /// <summary>
    /// 好友下线
    /// </summary>
    FriendOffline,

    /// <summary>
    /// 好友申请
    /// </summary>
    FriendApply,

    /// <summary>
    /// 新好友
    /// </summary>
    NewFriend,
}

public enum ChatMessageType : sbyte
{
    /// <summary> 文字 </summary>
    Text = 1,

    /// <summary> 图片 </summary>
    Image,

    /// <summary> 视频 </summary>
    Video,

    /// <summary> 其他文件 </summary>
    OtherFile
}

public enum ChatMessageSendType : sbyte
{
    /// <summary> 个人 </summary>
    Personal = 1,

    /// <summary> 群组 </summary>
    Group
}
