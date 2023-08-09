using Microsoft.EntityFrameworkCore;

namespace QianShiChatClient.Core.Common;

public interface IChatContext : IUnitOfWork
{
    DbSet<UserInfo> UserInfos { get; }
    DbSet<ChatMessage> ChatMessages { get; }
    DbSet<Session> Sessions { get; }
}
