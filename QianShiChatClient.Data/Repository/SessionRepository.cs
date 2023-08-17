namespace QianShiChatClient.Data.Repository;

public class SessionRepository : BaseRepository<Session>, ISessionRepository
{
    public SessionRepository(ChatDbContext context) : base(context)
    {
    }

    public Task<List<Session>> GetAllSessionAsync()
    {
        return GetAll().ToListAsync();
    }

    public async Task SaveSessionAsync(Session session)
    {
        var entity = await GetByIdAsync(session.Id);

        if (entity is null)
        {
            await AddAsync(session);
        }
        else
        {
            await UpdateAsync(session);
        }
    }

    public async Task SaveSessionAsync(IEnumerable<Session> sessions)
    {
        foreach (var session in sessions)
        {
            await SaveSessionAsync(session);
        }
    }
}
