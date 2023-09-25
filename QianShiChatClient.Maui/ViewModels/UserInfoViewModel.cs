namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class UserInfoViewModel : ViewModelBase
{
    private readonly DataCenter _dataCenter;
    private readonly IUserService _userService;

    [ObservableProperty]
    private UserInfoModel _info;

    public UserInfoViewModel(DataCenter dataCenter, IUserService userService)
    {
        _dataCenter = dataCenter;
        _userService = userService;
    }

    [RelayCommand]
    private async Task Send()
    {
        if (Info is null) return;
        var session = _dataCenter.Rooms.FirstOrDefault(x => x.ToId == Info.Id);

        // goto message page.
        var parameters = new Dictionary<string, object>
        {
            [nameof(MessageViewModel.CurrentSelectedRoom)] = session
        };
        await _navigationService.GoToMessagePage(parameters);
    }
}
