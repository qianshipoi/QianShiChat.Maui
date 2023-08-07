namespace QianShiChatClient.Core.Models;

public partial class FriendItem : ObservableObject
{
    public int Id { get; set; }

    public string Account { get; set; }

    [ObservableProperty]
    public string _nickName;

    [ObservableProperty]
    public string _avatar;

    [ObservableProperty]
    public DateTimeOffset _lastMessageTime;

    [ObservableProperty]
    public string _content;

    public ObservableCollection<ChatMessage> Messages { get; set; } = new ObservableCollection<ChatMessage>();
}
