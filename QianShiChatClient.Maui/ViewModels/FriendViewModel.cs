using QianShiChatClient.Application.IServices;

namespace QianShiChatClient.Maui.ViewModels;

public class OperationItem
{
    public string Name { get; set; }

    public ICommand Command { get; set; }
}

public sealed partial class FriendViewModel : ViewModelBase
{
    public DataCenter DataCenter { get; }
    private readonly IRoomRemoteService _roomRemoteService;

    public List<OperationItem> Operations { get; private set; }

    [ObservableProperty]
    private View _content;

    public FriendViewModel(DataCenter dataCenter, IRoomRemoteService roomRemoteService)
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
        _roomRemoteService = roomRemoteService;
    }

    [RelayCommand]
    private void OpenMenu()
    {
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Locked;
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
        Shell.Current.FlyoutIsPresented = true;
    }

    [RelayCommand]
    private async Task OpenNewWindow(UserInfoModel user)
    {
        if (MauiAppConsts.IsDesktop)
        {
            var roomDto = await _roomRemoteService.GetRoomAsync(user.Id, ChatMessageSendType.Personal);
            var room = new UserRoomModel(roomDto.Id, user);
            ServiceHelper.GetService<IWindowManagerService>().OpenChatRoomWindow(room);
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
            var roomDto = await _roomRemoteService.GetRoomAsync(user.Id, ChatMessageSendType.Personal);
            await _navigationService.GoToMessageDetailPage(roomDto.Id);
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