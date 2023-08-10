namespace QianShiChatClient.Application.Models;

public static class ModelToEntityExtenions
{
    public static Session ToSession(this SessionModel sessionModel)
    {
        return new Session
        {
            Id = sessionModel.Id,
            ToId = sessionModel.User.Id,
            Type = ChatMessageSendType.Personal,
            LastMessageContent = sessionModel.LastMessageContent,
            LastMessageTime = sessionModel.LastMessageTime
        };
    }

    public static UserInfo ToUserInfo(this UserInfoModel user)
    {
        return new UserInfo
        {
            Id = user.Id,
            Account = user.Account,
            Avatar = user.Avatar,
            Content = user.Content,
            CreateTime = user.CreateTime,
            NickName = user.NickName
        };
    }

    public static ChatMessage ToChatMessage(this ChatMessageModel message)
    {
        return new ChatMessage
        {
            Id = message.Id,
            Content = message.Content,
            CreateTime = message.CreateTime,
            FromId = message.FromId,
            IsSelfSend = message.IsSelfSend,
            LocalId = message.LocalId,
            MessageType = message.MessageType,
            SendType = message.SendType,
            Status = message.Status,
            ToId = message.ToId,
        };
    }

    public static ChatMessageModel ToChatMessageModel(this ChatMessage message)
    {
        return new ChatMessageModel
        {
            Id = message.Id,
            Content = message.Content,
            CreateTime = message.CreateTime,
            FromId = message.FromId,
            IsSelfSend = message.IsSelfSend,
            LocalId = message.LocalId,
            MessageType = message.MessageType,
            SendType = message.SendType,
            Status = message.Status,
            ToId = message.ToId,
        };
    }
}
