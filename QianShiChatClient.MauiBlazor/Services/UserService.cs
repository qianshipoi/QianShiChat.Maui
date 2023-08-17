namespace QianShiChatClient.Maui.Services;

public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IApiClient _client;
    private readonly IMemoryCache _memoryCache;
    private readonly IUserInfoRepository _userInfoRepository;

    public UserService(
        ILogger<UserService> logger, 
        IApiClient client, 
        IMemoryCache memoryCache, 
        IUserInfoRepository userInfoRepository)
    {
        _logger = logger;
        _client = client;
        _memoryCache = memoryCache;
        _userInfoRepository = userInfoRepository;
    }

    private string GetUserCacheKey(int id) => nameof(GetUserInfoByIdAsync) + id;

    public async Task<UserInfoModel> GetUserInfoByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetUserCacheKey(id);
        var userInfo = _memoryCache.Get<UserInfoModel>(cacheKey);

        if (userInfo is not null)
        {
            return userInfo;
        }

        var user = await _userInfoRepository.GetByIdAsync(id);
        if (user is null)
        {
            try
            {
                var serverUser = await _client.FindUserAsync(id, cancellationToken);
                if (serverUser is not null)
                {
                    userInfo = serverUser.ToUserInfoModel();
                    await _userInfoRepository.SaveUserAsync(userInfo.ToUserInfo());
                }
                else
                {
                    userInfo = UserInfoModel.Unknown;
                }
            }
            catch (Exception ex)
            {
                userInfo = UserInfoModel.Unknown;
                _logger.LogError(ex, "查询用户异常: id - {id}", id);
            }
        }

        _memoryCache.Set(cacheKey, userInfo, DateTimeOffset.Now.AddMinutes(2));
        return userInfo;
    }

    public UserInfoModel CurrentUser() => App.Current.User;
}