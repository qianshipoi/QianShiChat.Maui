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
        MainPage = ServiceProvider.GetRequiredService<LoginPage>();
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
        //var displayInfo = DeviceDisplay.Current.MainDisplayInfo;
        //window.X = (displayInfo.Width / displayInfo.Density - window.Width) / 2;
        //window.Y = (displayInfo.Height / displayInfo.Density - window.Height) / 2;

        return window;
    }
}