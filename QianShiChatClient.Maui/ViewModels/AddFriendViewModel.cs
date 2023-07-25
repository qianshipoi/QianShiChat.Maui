namespace QianShiChatClient.Maui.ViewModels;

[QueryProperty(nameof(UserInfo), nameof(UserInfo))]
public sealed partial class AddFriendViewModel : ViewModelBase
{
    private readonly IApiClient _apiClient;

    [ObservableProperty]
    private UserInfo _userInfo;

    public AddFriendViewModel(IApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    [RelayCommand]
    private async Task Send(string msg)
    {
        if (string.IsNullOrWhiteSpace(msg))
        {
            await Toast.Make(_stringLocalizer.GetString("VerificationInfoCanNotEmpty")).Show();
            return;
        }

        if (IsBusy) return;
        try
        {
            await _apiClient.FriendApplyAsync(new FriendApplyRequest(UserInfo.Id, msg));
            await Toast.Make(_stringLocalizer.GetString("SendSuccessed")).Show();
            await _navigationService.GoBack();
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
        finally
        {
            IsBusy = false;
        }
    }
}