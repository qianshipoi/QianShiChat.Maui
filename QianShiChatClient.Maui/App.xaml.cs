using QianShiChatClient.Application.IServices;

namespace QianShiChatClient.Maui;

public partial class App : Microsoft.Maui.Controls.Application
{
    public readonly IServiceProvider ServiceProvider;
    private readonly Settings _settings;

    public new static App Current => (App)Microsoft.Maui.Controls.Application.Current;

    public UserInfoModel User { get; set; }

    public App(IServiceProvider serviceProvider)
    {
        ServiceProvider = serviceProvider;
        ServiceHelper.Current = serviceProvider;
        InitializeComponent();
        _settings = ServiceProvider.GetRequiredService<Settings>();
        UserAppTheme = _settings.Theme switch { 
            Core.AppTheme.Light => Microsoft.Maui.ApplicationModel.AppTheme.Light,
            Core.AppTheme.Dark => Microsoft.Maui.ApplicationModel.AppTheme.Dark,
            _ => Microsoft.Maui.ApplicationModel.AppTheme.Unspecified,
        };
        var culture = new CultureInfo(_settings.Language);
        LocalizationResourceManager.Instance.SetCulture(culture);
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
        var screenHeight = Microsoft.UI.Windowing.DisplayArea.Primary.WorkArea.Height;
        var screenWidth = Microsoft.UI.Windowing.DisplayArea.Primary.WorkArea.Width;

        window.X = (screenWidth - width) / 2;
        window.Y = (screenHeight - height) / 2;
#endif

        window.Destroying += Window_Destroying;

        return window;
    }

    private void Window_Destroying(object sender, EventArgs e)
    {
        ServiceProvider.GetRequiredService<IWindowManagerService>().CloseAllWindow();
    }
}