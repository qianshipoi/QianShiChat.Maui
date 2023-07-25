namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class NewFriendViewModel : ViewModelBase
{
    public DataCenter DataCenter { get; }

    public NewFriendViewModel(
        INavigationService navigationService,
        IStringLocalizer<MyStrings> stringLocalizer,
        DataCenter dataCenter)
        : base(navigationService, stringLocalizer)
    {
        DataCenter = dataCenter;
    }

    [RelayCommand]
    private Task Reject(ApplyPending apply)
    {
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task Pass(ApplyPending apply)
    {
        return Task.CompletedTask;
    }

    [RelayCommand]
    private Task JoinNewFriendDetailPage(ApplyPending apply) => _navigationService.GoToNewFriendDetailPage(apply);
}