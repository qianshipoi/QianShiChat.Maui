namespace QianShiChatClient.MauiBlazor.Services;

/// <summary>
/// navigation service.
/// </summary>
public class NavigationService : INavigationService
{
    public Task GoToRoot() => Shell.Current.Navigation.PopToRootAsync();

    public Task GoBack() => Shell.Current.GoToAsync("..");

    public Task GoToSearchPage() => throw new NotSupportedException();

    public Task GoToQueryPage() => throw new NotSupportedException();

    public Task GoToMessageDetailPage(int sessionId)
    {
        throw new NotSupportedException();
    }

    public Task GoToSettingsPage() => throw new NotSupportedException();

    public Task GoToMessagePage() => throw new NotSupportedException();

    public Task GoToMessagePage(IDictionary<string, object> parameters) => throw new NotSupportedException();

    public Task GoToAddFriendPage(UserInfo user) => throw new NotSupportedException();

    public Task GoToLoginPage() => throw new NotSupportedException();

    public Task GoToNewFriendPage() => throw new NotSupportedException();

    public Task GoToNewFriendDetailPage(ApplyPending pending) => throw new NotSupportedException();

    public Task GoToScanningPage() => throw new NotSupportedException();

    public Task GoToQrAuthPage(string key) => throw new NotSupportedException();
}
