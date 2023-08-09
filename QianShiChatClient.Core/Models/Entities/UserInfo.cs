namespace QianShiChatClient.Core;

public class UserInfo
{
    public int Id { get; set; }
    public string Account { get; set; } = default!;
    public string NickName { get; set; } = default!;
    public string? Avatar { get; set; }
    public string? Content { get; set; }
    public bool IsFriend { get; set; }
}
