namespace QianShiChatClient.Core.Services;

public interface IUserService
{
    Task<UserInfo> GetUserInfoByIdAsync(int id, CancellationToken cancellationToken = default);

    UserInfo CurrentUser();
}