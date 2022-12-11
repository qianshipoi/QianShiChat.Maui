namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class AppShellViewModel : ViewModelBase
{
    public AppShellViewModel(
        ChatHub chatHub,
        INavigationService navigationService,
        IStringLocalizer<MyStrings> stringLocalizer)
        : base(navigationService, stringLocalizer)
    {
        _ = chatHub.Connect();
    }
}
