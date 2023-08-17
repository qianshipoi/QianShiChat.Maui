namespace QianShiChatClient.Data.Repository;

public class ChatMessageRepository : BaseRepository<ChatMessage>, IChatMessageRepository
{
    public ChatMessageRepository(ChatDbContext context) : base(context)
    {
    }

    public Task<List<ChatMessage>> GetChatMessageAsync(int userId, int page = 1, int size = 10)
    {
        throw new NotImplementedException();
    }

    public async Task SaveChatMessageAsnyc(ChatMessage message)
    {
        var user = await GetByIdAsync(message.LocalId);
        if (user is null)
            await AddAsync(message);
        else
            await UpdateAsync(message);
    }

    public async Task SaveChatMessagesAsnyc(IEnumerable<ChatMessage> messages)
    {
        foreach (var message in messages)
        {
            await SaveChatMessageAsnyc(message);
        }
    }

    public Task UpdateChatMessageAsync(ChatMessage message)
    {
        return SaveChatMessageAsnyc(message);
    }
}
