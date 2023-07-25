namespace QianShiChatClient.Maui.Services;

public interface IUserService
{
    Task<UserInfo> GetUserInfoByIdAsync(int id, CancellationToken cancellationToken = default);
}