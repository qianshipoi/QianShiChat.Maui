namespace QianShiChatClient.Maui.ViewModels;

[QueryProperty(nameof(Pending), nameof(Pending))]
public sealed partial class NewFriendDetailViewModel : ViewModelBase
{
    [ObservableProperty]
    private ApplyPending _pending;
}