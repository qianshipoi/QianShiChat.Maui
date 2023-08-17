using QianShiChatClient.MauiBlazor.Services;

namespace QianShiChatClient.MauiBlazor;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        builder.Services.ConfigureService();

        builder.Services.AddSingleton<WeatherForecastService>();

        return builder.Build();
    }


    public static IServiceCollection ConfigureService(this IServiceCollection services)
    {
        services.AddMemoryCache();

        services.AddApplication();
        services.AddMauiApplication();

        services.AddSingleton<IApiClient, ApiClient>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<ChatHub>();
        services.AddSingleton<DataCenter>();

        services.AddSingleton<SplashScreenPage>();
        services.AddSingleton<MainPage>();
        services.AddSingleton<LoginPage, LoginViewModel>();
        return services;
    }
}