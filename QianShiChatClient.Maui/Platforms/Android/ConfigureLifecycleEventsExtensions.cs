namespace QianShiChatClient.Maui.Platforms.Android
{
    public static class ConfigureLifecycleEventsExtensions
    {
        public static MauiAppBuilder ConfigureAndroidLifecycleEvents(this MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(events =>
            {
                events.AddAndroid(android =>
                {
                    android.OnResume(res =>
                    {
                        _ = ServiceHelper.Current.GetRequiredService<ChatHub>().Connect();
                    });
                });
            });

            return builder;
        }

        public static ILifecycleBuilder ConfigureAndroidLifecycleEvents(this ILifecycleBuilder builder)
        {
            builder.AddAndroid(android =>
            {
                android.OnResume(res =>
                {
                    if(App.Current.User != null && !string.IsNullOrWhiteSpace(Settings.AccessToken))
                    {
                        _ = ServiceHelper.Current.GetRequiredService<ChatHub>().Connect();
                        _ = ServiceHelper.Current.GetRequiredService<DataCenter>().GetUnreadMessageAsync();
                    }
                });
            });
            return builder;
        }
    }
}
