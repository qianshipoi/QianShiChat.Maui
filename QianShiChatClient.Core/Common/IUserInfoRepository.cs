namespace QianShiChatClient.Core.Common;

public interface IUserInfoRepository : IRepository<UserInfo>
{
    Task SaveUserAsync(UserInfo info);
}
