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

        var session = _dataCenter.Sessions.FirstOrDefault(x => x.User.Id == Info.Id);

        if (session is not null)
        {
            _dataCenter.Sessions.Remove(session);
        }
        else
        {
            session = new SessionModel(_userService, Info, new List<ChatMessageModel>());
        }

        _dataCenter.Sessions.Insert(0, session);

        // goto message page.
        var parameters = new Dictionary<string, object>
        {
            [nameof(MessageViewModel.CurrentSelectedSession)] = session
        };
        await _navigationService.GoToMessagePage(parameters);
    }
}
