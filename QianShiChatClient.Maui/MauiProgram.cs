namespace QianShiChatClient.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("iconfont.ttf", IconFontIcons.FontFamily);
            })
            .Services.ConfigureService(); ;

#if DEBUG
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}

    public static IServiceCollection ConfigureService(this IServiceCollection services)
    {
        services.AddLocalization();

        services.AddHttpClient<IApiClient, ApiClient>();
        services.AddSingleton<ChatHub>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<ChatDatabase>();
        services.AddSingleton<DataCenter>();

        services.AddTransient<AppShell, AppShellViewModel>();
        services.AddTransient<LoginPage, LoginViewModel>();
        services.AddTransient<MessagePage, MessageViewModel>();
        services.AddTransient<FriendPage, FriendViewModel>();
        services.AddTransient<SettingsPage, SettingsViewModel>();

        services.AddTransientWithShellRoute<MessageDetailPage, MessageDetailViewModel>(nameof(MessageDetailPage));
        services.AddTransientWithShellRoute<SearchPage, SearchViewModel>(nameof(SearchPage));
        services.AddTransientWithShellRoute<QueryPage, QueryViewModel>(nameof(QueryPage));
        services.AddTransientWithShellRoute<AddFriendPage, AddFriendViewModel>(nameof(AddFriendPage));

        return services;
    }

}
