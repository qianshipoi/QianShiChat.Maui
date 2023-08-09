using QianShiChatClient.Core;

namespace QianShiChatClient.Core;

public class Session
{
    public int Id { get; set; }
    public ChatMessageSendType Type { get; set; }
    public int ToId { get; set; }
    public long LastMessageTime { get; set; }
    public string? LastMessageContent { get; set; }
}
