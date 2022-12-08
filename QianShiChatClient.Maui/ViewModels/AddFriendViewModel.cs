﻿namespace QianShiChatClient.Maui.ViewModels;

[QueryProperty(nameof(UserInfo), nameof(UserInfo))]
public sealed partial class AddFriendViewModel : ViewModelBase
{
    readonly IApiClient _apiClient;

    [ObservableProperty]
    UserInfo _userInfo;

    public AddFriendViewModel(
        IStringLocalizer<MyStrings> stringLocalizer,
        INavigationService navigationService,
        IApiClient apiClient)
        : base(navigationService, stringLocalizer)
    {
        _apiClient = apiClient;
    }

    [RelayCommand]
    async Task Send(string msg)
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