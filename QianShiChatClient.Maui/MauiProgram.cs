﻿#if ANDROID

using QianShiChatClient.Maui.Platforms.Android;

#elif WINDOWS
using CommunityToolkit.Maui;

using QianShiChatClient.Maui.Platforms.Windows;
#endif

namespace QianShiChatClient.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseBarcodeReader()
            .UseMauiCommunityToolkit()
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
            })
            .ConfigureMopups(() => {
            })
            .Services.ConfigureService();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }

    public static IServiceCollection ConfigureService(this IServiceCollection services)
    {
        services.AddLocalization();
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
        services.AddSingleton<ChatHub>();
        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<ChatDatabase>();
        services.AddSingleton<DataCenter>();
        services.AddSingleton<IDialogService, DialogService>();

        services.AddSingleton<IUserService, UserService>();

        if (AppConsts.IsDesktop)
        {
            services.AddTransient<DesktopMessagePage, MessageViewModel>();
            services.AddTransient<DesktopShell, DesktopShellViewModel>();
            services.AddTransientWithShellRoute<SettingsPage, SettingsViewModel>(nameof(SettingsPage));
        }
        else
        {
            services.AddTransient<AppShell, AppShellViewModel>();
            services.AddTransient<MessagePage, MessageViewModel>();
            services.AddTransient<SettingsPage, SettingsViewModel>();
        }
        services.AddTransient<ChatMessageViewModel>();

        services.AddTransient<LoginPage, LoginViewModel>();
        services.AddTransient<FriendPage, FriendViewModel>();

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