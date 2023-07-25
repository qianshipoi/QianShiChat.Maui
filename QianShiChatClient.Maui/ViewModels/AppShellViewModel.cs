namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class AppShellViewModel : ViewModelBase
{
    private readonly ChatHub _chatHub;
    private readonly IDialogService _dialogService;

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
    private async Task Logout()
    {
        Settings.CurrentUser = null;
        Settings.AccessToken = null;
        App.Current.User = null;
        await _chatHub.Deconnect();
        await _navigationService.GoToLoginPage();
    }

    [RelayCommand]
    private Task MessageDialog() => _dialogService.PushMessageDialog();
}