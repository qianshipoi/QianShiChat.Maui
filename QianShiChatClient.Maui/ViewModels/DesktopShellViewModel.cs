using QianShiChatClient.Maui.Windows;

namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class DesktopShellViewModel : ViewModelBase
{
    private readonly ChatHub _chatHub;
    private readonly IDialogService _dialogService;

    public DesktopShellViewModel(
        ChatHub chatHub,
        IDialogService dialogService)
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
    private void MessageDialog()
    {

    }

    [RelayCommand]
    private Task GotoSettings() => _navigationService.GoToSettingsPage();
}