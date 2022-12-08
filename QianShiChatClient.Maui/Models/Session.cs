namespace QianShiChatClient.Maui.Models;

public partial class Session : ObservableObject
{
    [ObservableProperty]
    int _unreadCount;

    public UserInfo User { get; }

    public ObservableCollection<ChatMessage> Messages { get; }

    public long LastMessageTime { get; private set; }

    public string LastMessageConent { get; private set; }

    public Session(UserInfo user, IEnumerable<ChatMessage> messages)
    {
        User = user;
        var orderMessages = messages.OrderBy(x => x.CreateTime);
        Messages = new ObservableCollection<ChatMessage>(orderMessages);
        LastMessageTime = orderMessages.LastOrDefault()?.CreateTime ?? 0;
        UnreadCount += messages.Count();
        LastMessageConent = orderMessages.LastOrDefault()?.Content;
    }

    public void AddMessages(IEnumerable<ChatMessage> messages)
    {
        var orderMessages = messages.OrderBy(x => x.CreateTime);
        foreach (var message in orderMessages)
        {
            AddMessage(message);
        }
        LastMessageTime = orderMessages.Last().CreateTime;
        LastMessageConent = orderMessages.Last().Content;
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
        LastMessageConent = message.Content;
    }
}
