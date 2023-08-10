namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class UserInfoViewModel : ViewModelBase
{
    private readonly DataCenter _dataCenter;

    [ObservableProperty]
    private UserInfoModel _info;

    public UserInfoViewModel(DataCenter dataCenter)
    {
        _dataCenter = dataCenter;
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
            session = new SessionModel(Info, new List<ChatMessageModel>());
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
