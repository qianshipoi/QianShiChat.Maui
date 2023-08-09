namespace QianShiChatClient.Application.Helpers;

public static class ServiceHelper
{
    public static TService? GetService<TService>() => Current.GetService<TService>();

    public static TService GetReqiredService<TService>() where TService : notnull => Current.GetRequiredService<TService>();

    public static IServiceProvider Current =>
#if WINDOWS
            MauiWinUIApplication.Current?.Services!;
#elif ANDROID
        MauiApplication.Current?.Services!;
#elif IOS || MACCATALYST
            MauiUIApplicationDelegate.Current?.Services!;
#else
            null!;
#endif
}