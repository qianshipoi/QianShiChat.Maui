using QianShiChatClient.Application.IServices;

namespace QianShiChatClient.MauiBlazor.Views;

public partial class SplashScreenPage : ContentPage
{
    private readonly Settings _settings;

    public SplashScreenPage(Settings settings)
    {
        _settings = settings;
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
        if (string.IsNullOrEmpty(_settings.AccessToken))
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
            App.Current.User = user.ToUserInfoModel();
            _settings.CurrentUser = App.Current.User;
            App.Current.MainPage = ServiceHelper.GetService<MainPage>();
        });
    }
}