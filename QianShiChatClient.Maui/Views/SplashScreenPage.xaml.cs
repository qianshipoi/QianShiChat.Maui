namespace QianShiChatClient.Maui.Views;

public partial class SplashScreenPage : ContentPage
{
    public SplashScreenPage()
    {
        InitializeComponent();

        Loaded += SplashScreenPage_Loaded;
    }

    private void SplashScreenPage_Loaded(object sender, EventArgs e)
    {
        var backgroundWorker = new BackgroundWorker();
        backgroundWorker.DoWork += BackgroundWorker_DoWork;
        backgroundWorker.RunWorkerAsync();
    }

    private void GoToLoginPage()
    {
        Dispatcher.Dispatch(() => {
            var loginPage = ServiceHelper.GetService<LoginPage>();
            App.Current.MainPage = new NavigationPage(loginPage);
        });

    }

    private async void BackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
    {
        if (string.IsNullOrEmpty(Settings.AccessToken))
        {
            await Task.Delay(1000);
            GoToLoginPage();
            return;
        }

        var (succeeded, user) = await ServiceHelper.GetService<IApiClient>().CheckAccessToken();
        if (!succeeded)
        {
            GoToLoginPage();
            return;
        }
        await Task.Delay(3000);

        Dispatcher.Dispatch(() => {
            App.Current.User = user.ToUserInfo();
            Settings.CurrentUser = App.Current.User;
            App.Current.MainPage = AppConsts.IsDesktop ?
                ServiceHelper.GetService<DesktopShell>() :
                ServiceHelper.GetService<AppShell>();
        });
    }
}