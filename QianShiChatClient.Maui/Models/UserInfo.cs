namespace QianShiChatClient.Maui.Models;

public partial class UserInfo : ObservableObject
{
    [PrimaryKey]
    public int Id { get; set; }

    public string Account { get; set; }

    [ObservableProperty]
    private string _nickName;

    [ObservableProperty]
    private string _avatar = "default_avatar.png";

    [ObservableProperty]
    private string _content = "个性签名";

    [ObservableProperty]
    private bool _isUnknown = false;

    public static readonly UserInfo Unknown = new() { IsUnknown = true };
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