namespace QianShiChatClient.Maui.Services;

public class UserService : IUserService
{
    private readonly ChatDatabase _chatDatabase;
    private readonly ILogger<UserService> _logger;
    private readonly IApiClient _client;
    private readonly IMemoryCache _memoryCache;

    public UserService(ChatDatabase chatDatabase, ILogger<UserService> logger, IApiClient client, IMemoryCache memoryCache)
    {
        _chatDatabase = chatDatabase;
        _logger = logger;
        _client = client;
        _memoryCache = memoryCache;
    }

    public async Task<UserInfo> GetUserInfoByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var userInfo = _memoryCache.Get<UserInfo>(nameof(GetUserInfoByIdAsync));

        if (userInfo is not null)
        {
            return userInfo;
        }

        userInfo = await _chatDatabase.GetUserByIdAsync(id);
        if (userInfo is null)
        {
            try
            {
                var serverUser = await _client.FindUserAsync(id, cancellationToken);
                if (serverUser is not null)
                {
                    userInfo = serverUser.ToUserInfo();
                    await _chatDatabase.SaveUserAsync(userInfo);
                }
                else
                {
                    userInfo = UserInfo.Unknown;
                }
            }
            catch (Exception ex)
            {
                userInfo = UserInfo.Unknown;
                _logger.LogError(ex, "查询用户异常: id - {id}", id);
            }
        }

        _memoryCache.Set(nameof(GetUserInfoByIdAsync), userInfo, DateTimeOffset.Now.AddMinutes(2));
        return UserInfo.Unknown;
    }
}