using QianShiChatClient.Maui.Services;
using QianShiChatClient.MauiBlazor.Services;

using SkiaSharp.Views.Maui.Controls.Hosting;

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

        services.AddHttpClient(AppConsts.API_CLIENT_NAME, client => {
            client.BaseAddress = new Uri(AppConsts.API_BASE_URL);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "QianShiChatClient-Maui");
            client.DefaultRequestHeaders.Add("Client-Type", AppConsts.CLIENT_TYPE);
            if (!string.IsNullOrWhiteSpace(Settings.AccessToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Settings.AccessToken);
            }
        });

        services.AddSingleton<IApiClient, ApiClient>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<ChatHub>();
        services.AddSingleton<ChatDatabase>();
        services.AddSingleton<DataCenter>();

        services.AddSingleton<SplashScreenPage>();
        services.AddSingleton<MainPage>();
        services.AddSingleton<LoginPage, LoginViewModel>();
        return services;
    }
}