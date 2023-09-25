using QianShiChatClient.Application.IServices;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddOptions();

        services.AddSingleton<IApiClient, ApiClient>();
        services.AddSingleton<ChatHub>();
        services.AddSingleton<DataCenter>();
        services.AddSingleton<Settings>();
        services.AddSingleton<IRoomRemoteService, RoomHubService>();

        services.PostConfigure<ApiOptions>(options => {
            options.ClientName = AppConsts.API_CLIENT_NAME;
        });

        services.AddHttpClient(AppConsts.API_CLIENT_NAME, (provider, client) => {
            var options = provider.GetRequiredService<IOptions<ApiOptions>>();
            var settings = provider.GetRequiredService<Settings>();
            client.BaseAddress = new Uri(options.Value.BaseUrl);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.DefaultRequestHeaders.Add("User-Agent", "QianShiChatClient-Maui");
            client.DefaultRequestHeaders.Add("Client-Type", options.Value.ClientType);

            if (!string.IsNullOrWhiteSpace(settings.AccessToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", settings.AccessToken);
            }
        });

        return services;
    }
}
