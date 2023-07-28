namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class LoginViewModel : ViewModelBase
{
    private readonly IApiClient _apiClient;
    private readonly IDispatcher _dispatcher;

    private Timer _updateQrCodeTimer;
    private Timer _checkTimer;
    private string _key;

    [ObservableProperty]
    private bool _isAccountAuthMode = true;

    [ObservableProperty]
    private string _account = "qianshi";

    [ObservableProperty]
    private string _password = "123456";

    [ObservableProperty]
    private ImageSource _authQrCodeImage;

    [ObservableProperty]
    private UserInfo _user;

    public LoginViewModel(
        IDispatcher dispatcher,
        IApiClient apiClient
    )
    {
        _apiClient = apiClient;
        _dispatcher = dispatcher;
    }

    private void JoinMainPage(UserInfo user)
    {
        _dispatcher.Dispatch(() => {
            App.Current.User = user;
            Settings.CurrentUser = App.Current.User;
            App.Current.MainPage = AppConsts.IsDesktop ?
                ServiceHelper.GetService<DesktopShell>() :
                ServiceHelper.GetService<AppShell>();
        });
    }

    private async Task CheckAccessToken()
    {
        var accessToken = Settings.AccessToken;
        if (!string.IsNullOrWhiteSpace(Settings.AccessToken))
        {
            var user = Settings.CurrentUser;
            if (user != null)
            {
                JoinMainPage(user);
                // update user info.
                _ = Task.Run(async () => {
                    var (isSuccessed, userDto) = await _apiClient.CheckAccessToken();
                    if (isSuccessed)
                    {
                        _dispatcher.Dispatch(() => {
                            App.Current.User = userDto.ToUserInfo();
                            Settings.CurrentUser = App.Current.User;
                        });
                    }
                    else
                    {
                        await _navigationService.GoToLoginPage();
                    }
                });
            }
            else
            {
                var (isSuccessed, userDto) = await _apiClient.CheckAccessToken();
                if (isSuccessed)
                {
                    JoinMainPage(userDto.ToUserInfo());
                }
            }
        }
    }

    [RelayCommand]
    private async Task Submit()
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
            var (succeeded, user, message) = await _apiClient.LoginAsync(new LoginRequest(Account, Password.ToMd5()));
            if (!succeeded)
            {
                await Toast.Make(message).Show();
                return;
            }
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

    private async Task GenerateQrCodeImage()
    {
        _updateQrCodeTimer?.Dispose();

        // create auth key.
        var qrKeyResponse = await _apiClient.CreateQrKeyAsync();
        _key = qrKeyResponse.Key;

        // create auth qrcode.
        var qrcodeResponse = await _apiClient.CreateQrCodeAsync(
            new CreateQrCodeRequest { Key = _key, Qrimg = true }
        );
        MainThread.BeginInvokeOnMainThread(() => {
            AuthQrCodeImage = ImageSource.FromStream(() => {
                qrcodeResponse.Image = qrcodeResponse.Image.Replace("data:image/png;base64,", "");
                var bytes = Convert.FromBase64String(qrcodeResponse.Image);
                return new MemoryStream(bytes);
            });
        });

        _updateQrCodeTimer = new Timer(state => _ = GenerateQrCodeImage(), null, 60000, 60000);
    }

    private void ClearAuthTimer()
    {
        _checkTimer?.Dispose();
        _updateQrCodeTimer?.Dispose();
        User = null;
    }

    [RelayCommand]
    private async Task SwitchAuthMode()
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

    private async Task CheckAuthStatus()
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
            await MainThread.InvokeOnMainThreadAsync(async () => {
                await Toast
                    .Make(LocalizationResourceManager.Instance["LoginSuccessed"].ToString())
                    .Show();
            });
            JoinMainPage(checkResponse.User.ToUserInfo());
        }
    }
}