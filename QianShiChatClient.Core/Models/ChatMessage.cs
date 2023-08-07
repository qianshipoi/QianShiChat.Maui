namespace QianShiChatClient.Core.Models;

public partial class ChatMessage : ObservableObject
{
    [PrimaryKey]
    public Guid LocalId { get; set; }

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

    [ObservableProperty]
    private MessageStatus _status;

    [ObservableProperty]
    [property: Ignore]
    private double _uploadProgressValue;
}

public enum MessageStatus
{
    Sending,
    Successful,
    Failed
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