namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class LoginViewModel : ViewModelBase
{
    readonly IApiClient _apiClient;
    readonly IDispatcher _dispatcher;
    readonly IServiceProvider _serviceProvider;

    Timer _updateQrCodeTimer;
    Timer _checkTimer;
    string _key;

    [ObservableProperty]
    bool _isAccountAuthMode = true;

    [ObservableProperty]
    string _account = "qianshi";

    [ObservableProperty]
    string _password = "123456";

    [ObservableProperty]
    ImageSource _authQrCodeImage;

    [ObservableProperty]
    UserInfo _user;

    public LoginViewModel(
        IStringLocalizer<MyStrings> stringLocalizer,
        INavigationService navigationService,
        IDispatcher dispatcher,
        IApiClient apiClient,
        IServiceProvider serviceProvider
    ) : base(navigationService, stringLocalizer)
    {
        _apiClient = apiClient;
        _dispatcher = dispatcher;
        Task.Run(CheckAccessToken);
        _serviceProvider = serviceProvider;
    }

    void JoinMainPage(UserInfo user)
    {
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
                            //App.Current.User.Avatar = _apiClient.FormatFile(userDto.Avatar);
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
        if (IsBusy)
            return;

        if (string.IsNullOrEmpty(Account) || string.IsNullOrEmpty(Password))
        {
            await Toast
                .Make(
                    LocalizationResourceManager.Instance["AccountOrPasswordCanNotEmpty"].ToString()
                )
                .Show();
            return;
        }

        IsBusy = true;
        try
        {
            var user = await _apiClient.LoginAsync(new LoginReqiest(Account, Password.ToMd5()));
            await Toast
                .Make(LocalizationResourceManager.Instance["LoginSuccessed"].ToString())
                .Show();
            JoinMainPage(user.ToUserInfo());
        }
        finally
        {
            IsBusy = false;
        }
    }

    async Task GenerateQrCodeImage()
    {
        _updateQrCodeTimer?.Dispose();

        // create auth key.
        var qrKeyResponse = await _apiClient.CreateQrKeyAsync();
        _key = qrKeyResponse.Key;

        // create auth qrcode.
        var qrcodeResponse = await _apiClient.CreateQrCodeAsync(
            new CreateQrCodeRequest { Key = _key, Qrimg = true }
        );
        MainThread.BeginInvokeOnMainThread(() =>
        {
            AuthQrCodeImage = ImageSource.FromStream(() =>
            {
                qrcodeResponse.Image = qrcodeResponse.Image.Replace("data:image/png;base64,", "");
                var bytes = Convert.FromBase64String(qrcodeResponse.Image);
                return new MemoryStream(bytes);
            });
        });

        _updateQrCodeTimer = new Timer(state => _ = GenerateQrCodeImage(), null, 60000, 60000);
    }

    void ClearAuthTimer()
    {
        _checkTimer?.Dispose();
        _updateQrCodeTimer?.Dispose();
        User = null;
    }

    [RelayCommand]
    async Task SwitchAuthMode()
    {
        if (IsBusy)
            return;
        IsBusy = true;
        try
        {
            if (IsAccountAuthMode)
            {
                await GenerateQrCodeImage();
                // start check timer.
                _checkTimer = new Timer(state => _ = CheckAuthStatus(), null, 2000, 2000);
            }
            else
            {
                ClearAuthTimer();
            }
            IsAccountAuthMode = !IsAccountAuthMode;
        }
        finally
        {
            IsBusy = false;
        }
    }

    async Task CheckAuthStatus()
    {
        var checkResponse = await _apiClient.CheckQrKeyAsync(_key);

        if (checkResponse.Code == 800)
        {
            // qrcode expired.
            ClearAuthTimer();
            await GenerateQrCodeImage();
            _checkTimer = new Timer(state => _ = CheckAuthStatus(), null, 2000, 2000);
        }
        else if (checkResponse.Code == 802)
        {
            // authorizing.
            var user = checkResponse.User.ToUserInfo();
            //user.Avatar = _apiClient.FormatFile(user.Avatar);
            User = user;
        }
        else if (checkResponse.Code == 803)
        {
            // auth successed.
            ClearAuthTimer();
            Settings.AccessToken = checkResponse.AccessToken;
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Toast
                    .Make(LocalizationResourceManager.Instance["LoginSuccessed"].ToString())
                    .Show();
            });
            JoinMainPage(checkResponse.User.ToUserInfo());
        }
    }
}
