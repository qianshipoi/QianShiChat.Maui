namespace QianShiChatClient.Maui.Services;

public interface INavigationService
{
    Task GoToRoot();

    Task GoBack();

    Task GoToAddFriendPage(UserInfo user);

    Task GoToLoginPage();

    Task GoToMessageDetailPage(int sessionId);

    Task GoToNewFriendDetailPage(ApplyPending pending);

    Task GoToNewFriendPage();

    Task GoToQueryPage();

    Task GoToScanningPage();

    Task GoToSearchPage();

    Task GoToSettingsPage();

    Task GoToMessagePage();

    Task GoToQrAuthPage(string key);
}