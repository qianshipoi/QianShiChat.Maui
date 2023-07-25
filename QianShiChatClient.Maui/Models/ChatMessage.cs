namespace QianShiChatClient.Maui.Models;

public class ChatMessage
{
    [PrimaryKey]
    public long Id { get; set; }

    public int FromId { get; set; }

    [Ignore]
    public string FromAvatar { get; set; }

    public int ToId { get; set; }

    [Ignore]
    public string ToAvatar { get; set; }

    public ChatMessageSendType SendType { get; set; }

    public ChatMessageType MessageType { get; set; }

    public string Content { get; set; } = null!;

    public long CreateTime { get; set; }

    public bool IsSelfSend { get; set; }
}

public static class ChatMessageExtensions
{
    public static ChatMessage ToChatMessage(this ChatMessageDto dto)
    {
        return new ChatMessage
        {
            Id = dto.Id,
            Content = dto.Content,
            FromId = dto.FromId,
            CreateTime = dto.CreateTime,
            IsSelfSend = dto.IsSelfSend,
            MessageType = dto.MessageType,
            SendType = dto.SendType,
            ToId = dto.ToId,
        };
    }

    public static IEnumerable<ChatMessage> ToChatMessages(this IEnumerable<ChatMessageDto> dtos)
    {
        return dtos.Select(x => x.ToChatMessage());
    }
}
