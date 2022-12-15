namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class AppShellViewModel : ViewModelBase
{
    readonly ChatHub _chatHub;
    public AppShellViewModel(
        ChatHub chatHub,
        INavigationService navigationService,
        IStringLocalizer<MyStrings> stringLocalizer)
        : base(navigationService, stringLocalizer)
    {
        _ = chatHub.Connect();
        _chatHub = chatHub;
    }

    [RelayCommand]
    async Task Logout()
    {
        Settings.CurrentUser = null;
        Settings.AccessToken = null;
        App.Current.User = null;
        await _chatHub.Deconnect();
        await _navigationService.GoToLoginPage();
    }
}
