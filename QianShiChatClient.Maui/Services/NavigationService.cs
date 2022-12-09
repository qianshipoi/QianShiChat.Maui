namespace QianShiChatClient.Maui.Services;

/// <summary>
/// navigation service.
/// </summary>
public class NavigationService : INavigationService
{
    public Task GoBack() => Shell.Current.GoToAsync("..");
    public Task GoToSearchPage() => Shell.Current.GoToAsync(nameof(SearchPage));
    public Task GoToQueryPage() => Shell.Current.GoToAsync(nameof(QueryPage));
    public Task GoToMessageDetailPage(Session sesion)
    {
        var navigationParameter = new Dictionary<string, object>
        {
            { nameof(MessageDetailViewModel.SessionId), sesion.User.Id }
        };
        return Shell.Current.GoToAsync(nameof(MessageDetailPage), true, navigationParameter);
    }
    public Task GoToSettingsPage() => Shell.Current.GoToAsync($"//{nameof(SettingsPage)}");
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
        Settings.AccessToken = null;
        Settings.CurrentUser = null;
        return Task.CompletedTask;
    }
    public Task GoToNewFriendPage() => Shell.Current.GoToAsync(nameof(GoToNewFriendPage), true);
}
