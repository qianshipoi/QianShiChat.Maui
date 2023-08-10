namespace QianShiChatClient.Application.Data;

public class ChatDatabase
{
    private SQLiteAsyncConnection? Database;

    public async Task Init()
    {
        if (Database is not null)
            return;
        Database = new SQLiteAsyncConnection(AppConsts.DatabasePath, AppConsts.Flags);
        await Database.CreateTablesAsync<UserInfo, ChatMessage, Session>();
    }

    public async Task SaveSessionAsync(Session session)
    {
        await Database!.InsertOrReplaceAsync(session);
    }

    public async Task SaveSessionsAsync(IEnumerable<Session> sessions)
    {
        await Database!.RunInTransactionAsync(con => {
            con.DeleteAll<Session>();
            foreach (var item in sessions)
            {
                if(item.Id == 0)
                {
                    continue;
                }
                con.InsertOrReplace(item);
            }
            con.Commit();
        });
    }

    public async Task<List<Session>> GetAllSessionAsync()
    {
        return await Database!.Table<Session>().ToListAsync();
    }

    public async Task<List<ChatMessage>> GetChatMessageAsync(int userId, int skip = 0, int count = 10)
    {
        return await Database!.Table<ChatMessage>()
            .Where(x => x.ToId == userId || x.FromId == userId)
            .Where(x => x.SendType == ChatMessageSendType.Personal)
            .OrderByDescending(x => x.Id)
            .Skip(skip)
            .Take(count)
            .ToListAsync();
    }

    public async Task SaveChatMessageAsnyc(ChatMessage message)
    {
        await Database!.InsertOrReplaceAsync(message);
    }

    public async Task UpdateChatMessageAsync(ChatMessage message)
    {
        await Database!.UpdateAsync(message);
    }

    public async Task SaveChatMessagesAsnyc(IEnumerable<ChatMessage> messages)
    {
        foreach (var message in messages)
        {
            await Database!.InsertOrReplaceAsync(message);
        }
    }

    public async Task<UserInfo> GetUserByIdAsync(int id)
    {
        return await Database!.Table<UserInfo>().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> UserExistsAsync(int id)
    {
        return await Database!.Table<UserInfo>().Where(x => x.Id == id).CountAsync() > 0;
    }

    public async Task DeleteUserAsync(int id)
    {
        await Database!.Table<UserInfo>().DeleteAsync(x => x.Id == id);
    }

    public async Task SaveUserAsync(UserInfo user)
    {
        await Database!.InsertOrReplaceAsync(user);
    }
}