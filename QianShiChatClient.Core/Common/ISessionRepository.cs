namespace QianShiChatClient.Core.Common;

public interface ISessionRepository : IRepository<Session>
{
    Task<List<Session>> GetAllSessionAsync();
    Task SaveSessionAsync(Session session);

    Task SaveSessionAsync(IEnumerable<Session> sessions);
}

