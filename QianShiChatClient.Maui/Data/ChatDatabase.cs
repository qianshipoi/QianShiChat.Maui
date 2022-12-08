﻿namespace QianShiChatClient.Maui.Data;

public class ChatDatabase
{
    SQLiteAsyncConnection Database;

    async Task Init()
    {
        if (Database is not null)
            return;

        Database = new SQLiteAsyncConnection(AppConsts.DatabasePath, AppConsts.Flags);
        await Database.CreateTablesAsync<UserInfo, ChatMessage, SessionModel>();
    }

    public async Task SaveSessionAsync(SessionModel session)
    {
        await Init();
        await Database.InsertOrReplaceAsync(session);
    }

    public async Task<List<SessionModel>> GetAllSessionAsync()
    {
        await Init();
        return await Database.Table<SessionModel>().ToListAsync();
    }

    public async Task<List<ChatMessage>> GetChatMessageAsync(int userId, int skip = 0, int count = 10)
    {
        await Init();
        return await Database.Table<ChatMessage>()
            .Where(x => x.ToId == userId || x.FromId == userId)
            .Where(x => x.SendType == ChatMessageSendType.Personal)
            .OrderByDescending(x => x.Id)
            .Skip(skip)
            .Take(count)
            .ToListAsync();
    }

    public async Task SaveChatMessageAsnyc(ChatMessage message)
    {
        await Init();
        await Database.InsertOrReplaceAsync(message);
    }

    public async Task SaveChatMessagesAsnyc(IEnumerable<ChatMessage> messages)
    {
        await Init();
        foreach (var message in messages)
        {
            await Database.InsertOrReplaceAsync(message);
        }
    }

    public async Task<UserInfo> GetUserByIdAsync(int id)
    {
        await Init();
        return await Database.Table<UserInfo>().FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<bool> UserExistsAsync(int id)
    {
        await Init();
        return await Database.Table<UserInfo>().Where(x => x.Id == id).CountAsync() > 0;
    }

    public async Task DeleteUserAsync(int id)
    {
        await Init();
        await Database.Table<UserInfo>().DeleteAsync(x => x.Id == id);
    }

    public async Task SaveUserAsync(UserInfo user)
    {
        await Init();
        await Database.InsertOrReplaceAsync(user);
    }
}