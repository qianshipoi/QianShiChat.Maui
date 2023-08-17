namespace QianShiChatClient.Maui.ViewModels;

public class OperationItem
{
    public string Name { get; set; }

    public ICommand Command { get; set; }
}

public sealed partial class FriendViewModel : ViewModelBase
{
    public DataCenter DataCenter { get; }

    public List<OperationItem> Operations { get; private set; }

    [ObservableProperty]
    private View _content;

    public FriendViewModel(DataCenter dataCenter)
    {
        DataCenter = dataCenter;
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
    }

    [RelayCommand]
    private void OpenMenu()
    {
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Locked;
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
        Shell.Current.FlyoutIsPresented = true;
    }

    [RelayCommand]
    private void OpenNewWindow(UserInfoModel user)
    {
        if (MauiAppConsts.IsDesktop)
        {
            ServiceHelper.GetService<IWindowManagerService>().OpenChatRoomWindow(user);
        }
    }

    [RelayCommand]
    private Task JoinSearchPage() => _navigationService.GoToSearchPage();

    [RelayCommand]
    private Task JoinQueryPage() => _navigationService.GoToQueryPage();

    [RelayCommand]
    private Task JoinNewFriendPage()
    {
        if (MauiAppConsts.IsDesktop)
        {
            Content = ServiceHelper.GetService<NewFriendView>();
            return Task.CompletedTask;
        }
        else
        {
            return _navigationService.GoToNewFriendPage();
        }
    }

    [RelayCommand]
    private async Task JoinDetail(UserInfoModel user)
    {
        if (MauiAppConsts.IsDesktop)
        {
            var view = ServiceHelper.GetService<UserInfoView>();
            (view.BindingContext as UserInfoViewModel).Info = user;
            Content = view;
        }
        else
        {
            await _navigationService.GoToMessageDetailPage(user.Id);
        }
    }

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