namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddMauiApplication(this IServiceCollection services)
    {
        services.AddTransient<IGlobalDispatcher, MauiDispatcher>();
        services.AddTransient<IGlobalDispatcherTimer, MauiDispatcherTimer>();
        services.AddTransient<IPreferencesService, MauiPreferencesService>();

        services.Configure<ApiOptions>(options => {
            options.ClientType = MauiAppConsts.CLIENT_TYPE;
            options.BaseUrl = MauiAppConsts.API_BASE_URL;
        });

        return services;
    }
}
