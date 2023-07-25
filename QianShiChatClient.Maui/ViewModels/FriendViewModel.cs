namespace QianShiChatClient.Maui.ViewModels;

public class OperationItem
{
    public string Name { get; set; }

    public ICommand Command { get; set; }
}

public sealed partial class FriendViewModel : ViewModelBase
{
    public DataCenter DataCenter { get; }

    public ObservableCollection<FriendItem> Friends { get; set; }

    public List<OperationItem> Operations { get; private set; }

    [ObservableProperty]
    private View _content;

    public FriendViewModel(
        DataCenter dataCenter,
        IStringLocalizer<MyStrings> stringLocalizer,
        INavigationService navigationService)
        : base(navigationService, stringLocalizer)
    {
        DataCenter = dataCenter;
        Friends = new ObservableCollection<FriendItem>();
        Operations = new List<OperationItem>
        {
            new OperationItem
            {
                Name = "好友通知",
                Command = JoinNewFriendPageCommand
            },
            new OperationItem
            {
                Name = "群通知",
                Command = JoinNewFriendPageCommand
            }
        };
        foreach (var friend in FakerFriend.GetFriends(20))
        {
            Friends.Add(friend);
        }
    }

    [RelayCommand]
    private void OpenMenu()
    {
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Locked;
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
        Shell.Current.FlyoutIsPresented = true;
    }

    [RelayCommand]
    private Task JoinSearchPage() => _navigationService.GoToSearchPage();

    [RelayCommand]
    private Task JoinQueryPage() => _navigationService.GoToQueryPage();

    [RelayCommand]
    private Task JoinNewFriendPage() => _navigationService.GoToNewFriendPage();

    [RelayCommand]
    private Task JoinDetail(UserInfo user) => _navigationService.GoToMessageDetailPage(user.Id);

    [RelayCommand]
    private void OperationExecute(OperationItem item)
    {
        if (item.Command.CanExecute(null))
        {
            item.Command.Execute(null);
        }
    }

    [RelayCommand]
    private async Task UpdateFriends()
    {
        try
        {
            await DataCenter.GetAllFriendAsync();
        }
        finally
        {
            IsBusy = false;
        }
    }
}