using QianShiChatClient.Application.IServices;

namespace QianShiChatClient.Maui.Services;

/// <summary>
/// navigation service.
/// </summary>
public class NavigationService : INavigationService
{
    private readonly Settings _settings;

    public NavigationService(Settings settings)
    {
        _settings = settings;
    }

    public Task GoToRoot() => Shell.Current.Navigation.PopToRootAsync();

    public Task GoBack() => Shell.Current.GoToAsync("..");

    public Task GoToSearchPage() => Shell.Current.GoToAsync(nameof(SearchPage));

    public Task GoToQueryPage() => Shell.Current.GoToAsync(nameof(QueryPage));

    public Task GoToMessageDetailPage(int sessionId)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { nameof(MessageDetailViewModel.SessionId), sessionId }
        };
        return Shell.Current.GoToAsync(nameof(MessageDetailPage), true, navigationParameter);
    }

    public Task GoToSettingsPage() => Shell.Current.GoToAsync((MauiAppConsts.IsDesktop ? "" : "//") + nameof(SettingsPage));

    public Task GoToMessagePage() => Shell.Current.GoToAsync($"//{nameof(MessagePage)}");

    public Task GoToMessagePage(IDictionary<string, object> parameters) => Shell.Current.GoToAsync($"//{nameof(MessagePage)}", parameters);

    public Task GoToAddFriendPage(UserInfo user)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { nameof(AddFriendViewModel.UserInfo), user }
        };
        return Shell.Current.GoToAsync(nameof(AddFriendPage), true, navigationParameter);
    }

    public Task GoToLoginPage()
    {
        App.Current.MainPage = App.Current.ServiceProvider.GetRequiredService<LoginPage>();
        _settings.AccessToken = null;
        _settings.CurrentUser = null;
        return Task.CompletedTask;
    }

    public Task GoToNewFriendPage() => Shell.Current.GoToAsync(nameof(NewFriendPage), true);

    public Task GoToNewFriendDetailPage(ApplyPending pending)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { nameof(NewFriendDetailViewModel.Pending), pending }
        };
        return Shell.Current.GoToAsync(nameof(NewFriendDetailPage), true, navigationParameter);
    }

    public Task GoToScanningPage() => Shell.Current.GoToAsync(nameof(ScanningPage));

    public Task GoToQrAuthPage(string key)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { nameof(QrAuthViewModel.Key), key }
        };
        return Shell.Current.GoToAsync(nameof(QrAuthPage), true, navigationParameter);
    }
}