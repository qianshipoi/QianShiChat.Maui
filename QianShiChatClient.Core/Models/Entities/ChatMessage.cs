namespace QianShiChatClient.Core;

public class ChatMessage
{
    public Guid LocalId { get; set; }

    public long Id { get; set; }

    public int FromId { get; set; }

    public string? FromAvatar { get; set; }

    public int ToId { get; set; }

    public string? ToAvatar { get; set; }

    public ChatMessageSendType SendType { get; set; }

    public ChatMessageType MessageType { get; set; }

    public string Content { get; set; } = null!;

    public long CreateTime { get; set; }

    public bool IsSelfSend { get; set; }

    public MessageStatus Status { get; set; }
}
