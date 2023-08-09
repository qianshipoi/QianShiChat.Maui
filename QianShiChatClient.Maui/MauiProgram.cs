namespace QianShiChatClient.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .UseBarcodeReader()
            .UseMauiCommunityToolkit()
            .ConfigureMauiWorkarounds()
            .ConfigureMopups()
            .UseSimpleShell()
            .UseSimpleToolkit()
            .ConfigureLifecycleEvents(events => {
#if ANDROID
                events.ConfigureAndroidLifecycleEvents();
#elif WINDOWS
                events.ConfigureWindowsLifecycleEvents();
#endif
            })
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("iconfont.ttf", IconFontIcons.FontFamily);
                fonts.AddFont("Nunito-Regular.ttf", "NunitoRegular");
                fonts.AddFont("Nunito-Bold.ttf", "NunitoBold");
                fonts.AddFont("Nunito-SemiBold.ttf", "NunitoSemiBold");
            })
            .Services.ConfigureService();

#if DEBUG
        builder.Logging.AddDebug();
#endif

#if IOS || ANDROID
        builder.DisplayContentBehindBars();
#endif
#if ANDROID
        builder.SetDefaultStatusBarAppearance(Colors.Transparent, false);
#endif

        return builder.Build();
    }

    public static IServiceCollection ConfigureService(this IServiceCollection services)
    {
        services.AddLocalization();
        services.AddMemoryCache();
        services.AddChatDbContext(Path.Combine(FileSystem.AppDataDirectory, "chat.db3"));

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

        services.AddScoped<SplashScreenPage>();

        services.AddSingleton<IApiClient, ApiClient>();
        services.AddSingleton<ChatHub>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<ChatDatabase>();
        services.AddSingleton<DataCenter>();
#if WINDOWS
        services.AddSingleton<IWindowManagerService, WinUIManagerService>();
#endif
        services.AddSingleton<IDialogService, DialogService>();

        services.AddSingleton<IUserService, UserService>();

        if (AppConsts.IsDesktop)
        {
            services.AddTransient<DesktopMessagePage>();
            services.AddTransient<DesktopShell, DesktopShellViewModel>();
            services.AddTransientWithShellRoute<SettingsPage, SettingsViewModel>(nameof(SettingsPage));
            services.AddTransient<DesktopFriendPage>();
            services.AddTransient<ChatMessageViewModel>();
            services.AddSingleton<NewFriendView>();
            services.AddTransient<UserInfoViewModel>();
            services.AddSingleton<UserInfoView>();
            services.AddTransient<DesktopAddQueryPage, AddQueryViewModel>();
        }
        else
        {
            services.AddTransient<AppShell, AppShellViewModel>();
            services.AddTransient<MessagePage>();
            services.AddTransient<SettingsPage, SettingsViewModel>();
        }

        services.AddTransient<LoginPage, LoginViewModel>();
        //services.AddTransient<FriendPage, FriendViewModel>();
        services.AddTransient<MessageViewModel>();

        services.AddTransientWithShellRoute<FriendPage, FriendViewModel>(nameof(FriendPage));

        services.AddTransientWithShellRoute<MessageDetailPage, MessageDetailViewModel>(nameof(MessageDetailPage));
        services.AddTransientWithShellRoute<SearchPage, SearchViewModel>(nameof(SearchPage));
        services.AddTransientWithShellRoute<QueryPage, QueryViewModel>(nameof(QueryPage));
        services.AddTransientWithShellRoute<AddFriendPage, AddFriendViewModel>(nameof(AddFriendPage));
        services.AddTransientWithShellRoute<NewFriendPage, NewFriendViewModel>(nameof(NewFriendPage));
        services.AddTransientWithShellRoute<NewFriendDetailPage, NewFriendDetailViewModel>(nameof(NewFriendDetailPage));
        services.AddTransientWithShellRoute<ScanningPage, ScanningViewModel>(nameof(ScanningPage));
        services.AddTransientWithShellRoute<QrAuthPage, QrAuthViewModel>(nameof(QrAuthPage));

        return services;
    }
}