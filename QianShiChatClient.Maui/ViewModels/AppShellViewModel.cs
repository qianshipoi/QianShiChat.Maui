namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class AppShellViewModel : ViewModelBase
{
    readonly ChatHub _chatHub;
    readonly IDialogService _dialogService;
    public AppShellViewModel(
        ChatHub chatHub,
        INavigationService navigationService,
        IStringLocalizer<MyStrings> stringLocalizer,
        IDialogService dialogService)
        : base(navigationService, stringLocalizer)
    {
        _ = chatHub.Connect();
        _chatHub = chatHub;
        _dialogService = dialogService;
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

    [RelayCommand]
    Task MessageDialog() => _dialogService.PushMessageDialog();
}
