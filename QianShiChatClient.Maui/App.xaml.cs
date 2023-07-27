namespace QianShiChatClient.Maui;

public partial class App : Application
{
    public readonly IServiceProvider ServiceProvider;

    public new static App Current => (App)Application.Current;

    public UserInfo User { get; set; }

    public App(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        InitializeComponent();

        UserAppTheme = Settings.Theme;
        var culture = new CultureInfo(Settings.Language);
        LocalizationResourceManager.Instance.SetCulture(culture);
        _ = ServiceProvider.GetRequiredService<ChatDatabase>().Init();
        MainPage = ServiceProvider.GetRequiredService<SplashScreenPage>();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);
        const double minWidth = 840;
        const double minHeight = 540;
        const double width = 980;
        const double height = 560;

        window.Width = width;
        window.Height = height;
        window.MinimumWidth = minWidth;
        window.MinimumHeight = minHeight;

#if WINDOWS
        var screenHeight =  Microsoft.UI.Windowing.DisplayArea.Primary.WorkArea.Height;
        var screenWidth =  Microsoft.UI.Windowing.DisplayArea.Primary.WorkArea.Width;
        
        window.X = (screenWidth - width) / 2;
        window.Y = (screenHeight - height) / 2;
#endif

        window.Destroying += Window_Destroying;

        return window;
    }

    private void Window_Destroying(object sender, EventArgs e)
    {
        ServiceProvider.GetRequiredService<WindowManagerService>().CloseAllWindow();
    }
}