namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class LoginViewModel : ViewModelBase
{
    readonly IApiClient _apiClient;
    readonly IDispatcher _dispatcher;
    readonly IServiceProvider _serviceProvider;
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
        ChatHub chatHub,
        IServiceProvider serviceProvider)
        : base(navigationService, stringLocalizer)
    {
        _apiClient = apiClient;
        _dispatcher = dispatcher;
        _chatHub = chatHub;
        Task.Run(CheckAccessToken);
        _serviceProvider = serviceProvider;
    }

    void JoinMainPage(UserInfo user)
    {
        user.Avatar = _apiClient.FormatFile(user.Avatar);
        _dispatcher.Dispatch(() =>
        {
            App.Current.User = user;
            Settings.CurrentUser = App.Current.User;
            App.Current.MainPage = _serviceProvider.GetRequiredService<AppShell>();
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
                // update user info.
                _ = Task.Run(async () =>
                 {
                     var (isSuccessed, userDto) = await _apiClient.CheckAccessToken(accessToken);
                     if (isSuccessed)
                     {
                         _dispatcher.Dispatch(() =>
                         {
                             App.Current.User.Avatar = _apiClient.FormatFile(userDto.Avatar);
                             App.Current.User.NickName = userDto.NickName;
                             Settings.CurrentUser = App.Current.User;
                         });
                     }
                 });
            }
            else
            {
                var (isSuccessed, userDto) = await _apiClient.CheckAccessToken(accessToken);
                if (isSuccessed)
                {
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
