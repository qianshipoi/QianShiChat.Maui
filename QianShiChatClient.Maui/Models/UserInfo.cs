namespace QianShiChatClient.Maui.Models;

public partial class UserInfo : ObservableObject
{
    [PrimaryKey]
    public int Id { get; set; }

    public string Account { get; set; }

    [ObservableProperty]
    string _nickName;

    [ObservableProperty]
    string _avatar = "default_avatar.png";

    [ObservableProperty]
    string _content = "个性签名";
}

public static class UserInfoExtensions
{
    public static UserInfo ToUserInfo(this UserDto user)
    {
        return new UserInfo
        {
            Account = user.Account,
            NickName = user.NickName,
            Id = user.Id,
            Avatar = user.Avatar,
        };
    }
}
