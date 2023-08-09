namespace QianShiChatClient.Application.Models;

public class SessionModel
{
    [PrimaryKey]
    public int Id { get; set; }

    [Required]
    public ChatMessageSendType Type { get; set; }

    [Required]
    public int ToId { get; set; }

    public long LastMessageTime { get; set; }

    public string? LastMessageContent { get; set; }
}

public static class SessionModelExtensions
{
    public static SessionModel ToSessionModel(this Session session)
    {
        return new SessionModel
        {
            Type = ChatMessageSendType.Personal,
            LastMessageContent = session.LastMessageContent,
            LastMessageTime = session.LastMessageTime,
            ToId = session.User.Id,
            Id = session.User.Id
        };
    }
}