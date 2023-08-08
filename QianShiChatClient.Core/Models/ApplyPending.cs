namespace QianShiChatClient.Core.Models;

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

public static class ApplyPendingExtensions
{
    public static ApplyPending ToApplyPending(this ApplyPendingDto dto)
    {
        return new ApplyPending
        {
            Id = dto.Id,
            UserId = dto.UserId,
            FriendId = dto.FriendId,
            CreateTime = dto.CreateTime,
            Status = dto.Status,
            Remark = dto.Remark,
            User = dto.User.ToUserInfo(),
            Friend = dto.Friend.ToUserInfo()
        };
    }
}