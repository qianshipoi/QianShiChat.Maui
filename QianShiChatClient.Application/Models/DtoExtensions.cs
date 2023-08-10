namespace QianShiChatClient.Application.Models;

public static class UserDtoExtensions
{
    public static UserInfo ToUserInfo(this UserDto user)
    {
        return new UserInfo
        {
            Account = user.Account,
            NickName = user.NickName,
            Id = user.Id,
            Avatar = user.Avatar,
            CreateTime = user.CreateTime,
        };
    }

    public static UserInfoModel ToUserInfoModel(this UserDto user)
    {
        return new UserInfoModel
        {
            Account = user.Account,
            NickName = user.NickName,
            Id = user.Id,
            Avatar = user.Avatar,
            CreateTime = user.CreateTime,
        };
    }
}

public static class ChatMessageDtoExtensions
{
    public static ChatMessage ToChatMessage(this ChatMessageDto dto)
    {
        return new ChatMessage
        {
            Id = dto.Id,
            Content = dto.Content,
            FromId = dto.FromId,
            CreateTime = dto.CreateTime,
            IsSelfSend = dto.IsSelfSend,
            MessageType = dto.MessageType,
            SendType = dto.SendType,
            ToId = dto.ToId,
        };
    }

    public static ChatMessageModel ToChatMessageModel(this ChatMessageDto dto)
    {
        return new ChatMessageModel
        {
            Id = dto.Id,
            Content = dto.Content,
            FromId = dto.FromId,
            CreateTime = dto.CreateTime,
            IsSelfSend = dto.IsSelfSend,
            MessageType = dto.MessageType,
            SendType = dto.SendType,
            ToId = dto.ToId,
        };
    }

    public static IEnumerable<ChatMessage> ToChatMessages(this IEnumerable<ChatMessageDto> dtos)
    {
        return dtos.Select(x => x.ToChatMessage());
    }

    public static IEnumerable<ChatMessageModel> ToChatMessageModels(this IEnumerable<ChatMessageDto> dtos)
    {
        return dtos.Select(x => x.ToChatMessageModel());
    }
}

public static class ApplyPendingDtoExtensions
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