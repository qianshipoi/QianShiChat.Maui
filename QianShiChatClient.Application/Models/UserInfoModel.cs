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
    [ObservableProperty]
    private bool _isOnline;
    public long CreateTime { get; set; }
    public static readonly UserInfoModel Unknown = new() { IsUnknown = true };
}

public partial class GroupModel : ObservableObject
{
    public int Id { get; set; }
    public int UserId { get; set; }
    [ObservableProperty]
    private string _name = string.Empty;
    [ObservableProperty]
    private string _avatar = "default_avatar.png";
    [ObservableProperty]
    private int _totalUser;
    [ObservableProperty]
    private long _createTime;
    public ObservableCollection<UserInfoModel> Users { get; }

    public GroupModel()
    {
        Users = new ObservableCollection<UserInfoModel>();
    }
}
