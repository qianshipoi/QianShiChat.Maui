namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class MessageViewModel : ViewModelBase
{
    readonly ChatHub _chatHub;
    public DataCenter DataCenter { get; }

    public MessageViewModel(
        IStringLocalizer<MyStrings> stringLocalizer,
        INavigationService navigationService,
        DataCenter dataCenter,
        ChatHub chatHub)
        : base(navigationService, stringLocalizer)
    {
        DataCenter = dataCenter;
        _chatHub = chatHub;
    }

    [RelayCommand]
    Task JoinDetail(Session item) => _navigationService.GoToMessageDetailPage(item.User.Id);

    [RelayCommand]
    Task JoinSearchPage() => _navigationService.GoToSearchPage();

    [RelayCommand]
    Task JoinQueryPage() => _navigationService.GoToQueryPage();

    [RelayCommand]
    async Task UpdateMessage()
    {
        try
        {
            await DataCenter.GetUnreadMessageAsync();
            await _chatHub.Connect();
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    void OpenMenu()
    {
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Locked;
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
        Shell.Current.FlyoutIsPresented = true;
    }
}
