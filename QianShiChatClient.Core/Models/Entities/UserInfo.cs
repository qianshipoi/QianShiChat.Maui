namespace QianShiChatClient.Core;

public class UserInfo
{
    public int Id { get; set; }
    public string Account { get; set; } = default!;
    public string? NickName { get; set; }
    public string? Avatar { get; set; }
    public string? Content { get; set; }
    public long CreateTime { get; set; }
}
