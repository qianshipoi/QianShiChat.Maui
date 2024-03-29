﻿namespace QianShiChatClient.Application.IServices;

public interface INavigationService
{
    Task GoToRoot();

    Task GoBack();

    Task GoToAddFriendPage(UserInfo user);

    Task GoToLoginPage();

    Task GoToMessageDetailPage(string roomId);

    Task GoToNewFriendDetailPage(ApplyPending pending);

    Task GoToNewFriendPage();

    Task GoToQueryPage();

    Task GoToScanningPage();

    Task GoToSearchPage();

    Task GoToSettingsPage();

    Task GoToMessagePage();

    Task GoToQrAuthPage(string key);
    Task GoToMessagePage(IDictionary<string, object> parameters);
}