namespace QianShiChatClient.Application.Models;

public partial class UserInfoModel : ObservableObject
{
    public int Id { get; set; }
    public string Account { get; set; } = default!;
    [ObservableProperty]
    private string? _nickName;
    [ObservableProperty]
    private string _avatar = "default_avatar.png";
    [ObservableProperty]
    private string _content = "个性签名";
    [ObservableProperty]
    private bool _isUnknown = false;
    [ObservableProperty]
    private bool _isFriend;
    public long CreateTime { get; set; }
    public static readonly UserInfoModel Unknown = new() { IsUnknown = true };
}

