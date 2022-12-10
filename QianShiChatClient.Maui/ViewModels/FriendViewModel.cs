using System.Windows.Input;

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

    public FriendViewModel(
        DataCenter dataCenter,
        IStringLocalizer<MyStrings> stringLocalizer,
        INavigationService navigationService)
        : base(navigationService, stringLocalizer)
    {
        DataCenter = dataCenter;
        Friends = new ObservableCollection<FriendItem>();
        Operations = new List<OperationItem>();
        Operations.Add(new OperationItem
        {
             Name = _stringLocalizer["NewFriend"],
             Command = JoinNewFriendPageCommand
        });
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

    [RelayCommand]
    Task JoinDetail(UserInfo user) => _navigationService.GoToMessageDetailPage(user.Id);

    [RelayCommand]
    void OperationExecute(OperationItem item)
    {
        if (item.Command.CanExecute(null))
        {
            item.Command.Execute(null);
        }
    }

    [RelayCommand]
    async Task UpdateFriends()
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
