namespace QianShiChatClient.Maui.Platforms.Windows
{
    public static class ConfigureLifecycleEventsExtensions
    {
        public static MauiAppBuilder ConfigureWindowsLifecycleEvents(this MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(events =>
            {
                events.AddWindows(windows =>
                {
                    windows.OnResumed(window =>
                    {
                        _ = ServiceHelper.Current.GetRequiredService<ChatHub>().Connect();
                    });
                });
            });

            return builder;
        }

        public static ILifecycleBuilder ConfigureWindowsLifecycleEvents(this ILifecycleBuilder builder)
        {
            builder.AddWindows(windows =>
            {
                windows.OnResumed(window =>
                {
                    _ = ServiceHelper.Current.GetRequiredService<ChatHub>().Connect();
                    _ = ServiceHelper.Current.GetRequiredService<DataCenter>().GetUnreadMessageAsync();
                });
            });
            return builder;
        }
    }
}
