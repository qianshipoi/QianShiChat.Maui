namespace QianShiChatClient.Data.Repository;

public class UserInfoRepository : BaseRepository<UserInfo>, IUserInfoRepository
{
    public UserInfoRepository(ChatDbContext context) : base(context)
    {
    }

    public async Task SaveUserAsync(UserInfo info)
    {
        var user = await GetByIdAsync(info.Id);
        if (user is null)
            await AddAsync(info);
        else
            await UpdateAsync(info);
    }
}
