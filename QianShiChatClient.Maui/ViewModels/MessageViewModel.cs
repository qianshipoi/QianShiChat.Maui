namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class MessageViewModel : ViewModelBase
{
    public DataCenter DataCenter { get; }

    public MessageViewModel(
        IStringLocalizer<MyStrings> stringLocalizer,
        INavigationService navigationService,
        DataCenter dataCenter)
        : base(navigationService, stringLocalizer)
    {
        DataCenter = dataCenter;
    }

    [RelayCommand]
    Task JoinDetail(Session item) => _navigationService.GoToMessageDetailPage(item);

    [RelayCommand]
    Task JoinSearchPage() => _navigationService.GoToSearchPage();

    [RelayCommand]
    Task JoinQueryPage() => _navigationService.GoToQueryPage();

    [RelayCommand]
    void OpenMenu()
    {
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Locked;
        Shell.Current.FlyoutBehavior = FlyoutBehavior.Flyout;
        Shell.Current.FlyoutIsPresented = true;
    }
}
