namespace QianShiChatClient.Maui.Models;

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

public class FakerFriend
{
    public static IEnumerable<FriendItem> GetFriends(int count)
    {
        Randomizer.Seed = new Random(1234);
        var friendGrenerator = new Faker<FriendItem>("zh_CN")
            .RuleFor(x => x.Id, f => f.UniqueIndex)
            .RuleFor(x => x.Account, f => f.Internet.Email())
            .RuleFor(x => x.NickName, f => f.Name.FullName())
            .RuleFor(x => x.Avatar, f => f.Internet.Avatar())
            .RuleFor(x => x.LastMessageTime, f => f.Date.Future())
            .RuleFor(x => x.Content, f => f.Lorem.Text());

        return friendGrenerator.Generate(count);
    }
}