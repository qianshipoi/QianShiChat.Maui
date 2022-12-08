namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class AppShellViewModel : ViewModelBase
{
    public AppShellViewModel(
        INavigationService navigationService, 
        IStringLocalizer<MyStrings> stringLocalizer) 
        : base(navigationService, stringLocalizer)
    {
    }
}
