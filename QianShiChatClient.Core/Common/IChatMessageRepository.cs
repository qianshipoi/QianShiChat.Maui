namespace QianShiChatClient.Core.Common;

public interface IChatMessageRepository : IRepository<ChatMessage>
{
    Task SaveChatMessageAsnyc(ChatMessage message);

    Task SaveChatMessagesAsnyc(IEnumerable<ChatMessage> message);

    Task UpdateChatMessageAsync(ChatMessage message);

    Task<List<ChatMessage>> GetChatMessageAsync(int userId, int page = 1, int size = 10);
}

