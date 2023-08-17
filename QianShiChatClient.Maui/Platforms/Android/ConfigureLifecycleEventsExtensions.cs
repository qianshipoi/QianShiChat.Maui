namespace QianShiChatClient.Maui.Platforms.Android
{
    public static class ConfigureLifecycleEventsExtensions
    {
        public static MauiAppBuilder ConfigureAndroidLifecycleEvents(this MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(events => {
                events.AddAndroid(android => {
                    android.OnResume(res => {
                        _ = App.Current.ServiceProvider.GetRequiredService<ChatHub>().Connect();
                    });
                });
            });

            return builder;
        }

        public static ILifecycleBuilder ConfigureAndroidLifecycleEvents(this ILifecycleBuilder builder)
        {
            builder.AddAndroid(android => {
                android.OnResume(res => {
                    var settings = App.Current.ServiceProvider.GetRequiredService<Settings>();
                    if (App.Current.User != null && !string.IsNullOrWhiteSpace(settings.AccessToken))
                    {
                        _ = App.Current.ServiceProvider.GetRequiredService<ChatHub>().Connect();
                        _ = App.Current.ServiceProvider.GetRequiredService<DataCenter>().GetUnreadMessageAsync();
                    }
                });
            });
            return builder;
        }
    }
}