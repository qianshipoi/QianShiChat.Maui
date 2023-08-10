namespace QianShiChatClient.Application.Services;

public interface IUserService
{
    Task<UserInfoModel> GetUserInfoByIdAsync(int id, CancellationToken cancellationToken = default);

    UserInfoModel CurrentUser();
}