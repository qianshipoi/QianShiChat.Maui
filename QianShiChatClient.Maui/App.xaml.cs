using Microsoft.Extensions.DependencyInjection;

namespace QianShiChatClient.Maui;

public partial class App : Application
{
    public readonly IServiceProvider ServiceProvider;

    public static new App Current => (App)Application.Current;

    public UserInfo User { get; set; }

    public App(IServiceProvider serviceProvider)
	{
        ServiceProvider = serviceProvider;
        InitializeComponent();

        UserAppTheme = Settings.Theme;
        var culture = new CultureInfo(Settings.Language);
        LocalizationResourceManager.Instance.SetCulture(culture);

        MainPage = serviceProvider.GetRequiredService<LoginPage>();
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);

        const double width = 400;
        const double height = 720;

        window.Width = width;
        window.Height = height;
        window.MaximumWidth = width;
        window.MaximumHeight = height;
        window.MinimumHeight = height;
        window.MinimumWidth = width;

        return window;
    }
}
