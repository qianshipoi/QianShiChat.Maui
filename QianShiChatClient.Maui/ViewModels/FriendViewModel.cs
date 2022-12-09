namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class FriendViewModel : ViewModelBase
{
    public ObservableCollection<FriendItem> Friends { get; set; }

    public FriendViewModel(
        IStringLocalizer<MyStrings> stringLocalizer,
        INavigationService navigationService)
        : base(navigationService, stringLocalizer)
    {
        Friends = new ObservableCollection<FriendItem>();
        foreach (var friend in FakerFriend.GetFriends(20))
        {
            Friends.Add(friend);
        }
    }

    [RelayCommand]
    void OpenMenu()
    {
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Locked;
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
        Shell.Current.FlyoutIsPresented = true;
    }

    [RelayCommand]
    Task JoinSearchPage() => _navigationService.GoToSearchPage();

    [RelayCommand]
    Task JoinQueryPage() => _navigationService.GoToQueryPage();

    [RelayCommand]
    Task JoinNewFriendPage() => _navigationService.GoToNewFriendPage();
}
