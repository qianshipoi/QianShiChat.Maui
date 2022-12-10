namespace QianShiChatClient.Maui.ViewModels;

[QueryProperty(nameof(Pending),nameof(Pending))]
public sealed partial class NewFriendDetailViewModel : ViewModelBase
{
    [ObservableProperty]
    ApplyPending _pending;

    public NewFriendDetailViewModel(
        INavigationService navigationService, 
        IStringLocalizer<MyStrings> stringLocalizer) 
        : base(navigationService, stringLocalizer)
    {
    }
}