namespace QianShiChatClient.Application.Models;

public class ApplyPending
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int FriendId { get; set; }
    public long CreateTime { get; set; }
    public int Status { get; set; }
    public string? Remark { get; set; }
    public UserInfo? User { get; set; }
    public UserInfo? Friend { get; set; }
}
