namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class AppShellViewModel : ViewModelBase
{
    private readonly ChatHub _chatHub;
    private readonly IDialogService _dialogService;
    private readonly Settings _settings;

    public AppShellViewModel(ChatHub chatHub, IDialogService dialogService, Settings settings)
    {
        _chatHub = chatHub;
        _dialogService = dialogService;
        _settings = settings;
        _ = chatHub.Connect();
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
    private Task MessageDialog() => _dialogService.PushMessageDialog();
}