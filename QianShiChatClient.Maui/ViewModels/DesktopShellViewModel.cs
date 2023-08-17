namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class DesktopShellViewModel : ViewModelBase
{
    private readonly ChatHub _chatHub;
    private readonly IDialogService _dialogService;
    private readonly Settings _settings;

    public DesktopShellViewModel(
        ChatHub chatHub,
        IDialogService dialogService,
        Settings settings)
    {
        _chatHub = chatHub;
        _dialogService = dialogService;
        _ = _chatHub.Connect();
        _settings = settings;
    }

    [RelayCommand]
    private async Task Logout()
    {
        _settings.CurrentUser = null;
        _settings.AccessToken = null;
        App.Current.User = null;
        await _chatHub.Deconnect();
        await _navigationService.GoToLoginPage();
    }

    [RelayCommand]
    private void MessageDialog()
    {

    }

    [RelayCommand]
    private Task GotoSettings() => _navigationService.GoToSettingsPage();
}