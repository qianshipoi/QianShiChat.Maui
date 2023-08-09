using QianShiChatClient.Core;
using QianShiChatClient.Core.Common;

namespace QianShiChatClient.Data.Repository;

public class UserInfoRepository : BaseRepository<UserInfo>, IUserInfoRepository
{
    public UserInfoRepository(ChatDbContext context) : base(context)
    {
    }
}
