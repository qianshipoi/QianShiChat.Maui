namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class LoginViewModel : ViewModelBase
{
    readonly IApiClient _apiClient;
    readonly IDispatcher _dispatcher;
    readonly ChatHub _chatHub;

    [ObservableProperty]
    string _account = "qianshi";

    [ObservableProperty]
    string _password = "123456";

    public LoginViewModel(
        IStringLocalizer<MyStrings> stringLocalizer,
        INavigationService navigationService,
        IDispatcher dispatcher,
        IApiClient apiClient,
        ChatHub chatHub)
        : base(navigationService, stringLocalizer)
    {
        _apiClient = apiClient;
        _dispatcher = dispatcher;
        _chatHub = chatHub;
        Task.Run(CheckAccessToken);
    }

    void JoinMainPage(UserInfo user)
    {
        _dispatcher.Dispatch(() =>
        {
            App.Current.User = user;
            App.Current.MainPage = new AppShell(_chatHub);
        });
    }

    async Task CheckAccessToken()
    {
        var accessToken = Settings.AccessToken;
        if (!string.IsNullOrWhiteSpace(Settings.AccessToken))
        {
            var user = Settings.CurrentUser;
            if (user != null)
            {
                JoinMainPage(user);
            }
            else
            {
                var (isSuccessed, userDto) = await _apiClient.CheckAccessToken(accessToken);
                if (isSuccessed)
                {
                    Settings.CurrentUser = App.Current.User;
                    JoinMainPage(userDto.ToUserInfo());
                }
            }
        }
    }

    [RelayCommand]
    async Task Submit()
    {
        if (IsBusy) return;

        if (string.IsNullOrEmpty(Account) || string.IsNullOrEmpty(Password))
        {
            await Toast.Make("请完善信息！").Show();
            return;
        }

        IsBusy = true;
        try
        {
            var user = await _apiClient.LoginAsync(new LoginReqiest(Account, Password.ToMd5()));
            await Toast.Make("登录成功").Show();
            JoinMainPage(user.ToUserInfo());
        }
        finally
        {
            IsBusy = false;
        }
    }
}
