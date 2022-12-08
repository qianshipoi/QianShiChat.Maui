﻿namespace QianShiChatClient.Maui.Services;

public interface INavigationService
{
    Task GoBack();
    Task GoToAddFriendPage(UserInfo user);
    Task GoToLoginPage();
    Task GoToMessageDetailPage(Session sesion);
    Task GoToQueryPage();
    Task GoToSearchPage();
    Task GoToSettingsPage();
}