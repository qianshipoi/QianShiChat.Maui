namespace QianShiChatClient.Maui.Models;

public partial class Session : ObservableObject
{
    [ObservableProperty]
    int _unreadCount;

    [ObservableProperty]
    long _lastMessageTime;

    [ObservableProperty]
    string _lastMessageContent;

    public UserInfo User { get; }

    public ObservableCollection<ChatMessage> Messages { get; }

    public Session(UserInfo user, IEnumerable<ChatMessage> messages)
    {
        User = user;
        var orderMessages = messages.OrderBy(x => x.CreateTime);
        Messages = new ObservableCollection<ChatMessage>(orderMessages);
        LastMessageTime = orderMessages.LastOrDefault()?.CreateTime ?? 0;
        UnreadCount += messages.Count();
        LastMessageContent = orderMessages.LastOrDefault()?.Content;
    }

    public void AddMessages(IEnumerable<ChatMessage> messages)
    {
        var orderMessages = messages.OrderBy(x => x.CreateTime);
        foreach (var message in orderMessages)
        {
            AddMessage(message);
        }
        LastMessageTime = orderMessages.Last().CreateTime;
        LastMessageContent = orderMessages.Last().Content;
    }

    public void AddMessage(ChatMessage message)
    {
        if (Messages.Any(x => x.Id == message.Id))
        {
            return;
        }
        Messages.Add(message);
        UnreadCount = message.IsSelfSend ? 0 : UnreadCount + 1;
        LastMessageTime = message.CreateTime;
        LastMessageContent = message.Content;
    }
}
