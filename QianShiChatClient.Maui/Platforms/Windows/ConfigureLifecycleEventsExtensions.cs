namespace QianShiChatClient.Maui.Platforms.Windows
{
    public static class ConfigureLifecycleEventsExtensions
    {
        public static MauiAppBuilder ConfigureWindowsLifecycleEvents(this MauiAppBuilder builder)
        {
            builder.ConfigureLifecycleEvents(events => {
                events.AddWindows(windows => {
                    windows.OnResumed(window => {
                        _ = ServiceHelper.Current.GetRequiredService<ChatHub>().Connect();
                    });
                });
            });

            return builder;
        }

        public static ILifecycleBuilder ConfigureWindowsLifecycleEvents(this ILifecycleBuilder builder)
        {
            builder.AddWindows(windows => {
                windows.OnResumed(window => {
                    //if (App.Current.User != null && !string.IsNullOrWhiteSpace(Settings.AccessToken))
                    //{
                    //    _ = ServiceHelper.Current.GetRequiredService<ChatHub>().Connect();
                    //    _ = ServiceHelper.Current.GetRequiredService<DataCenter>().GetUnreadMessageAsync();
                    //}
                })
                .OnWindowCreated(window => {
                    //var mauiwin = window as MauiWinUIWindow;
                    //if (mauiwin is null) return;

                    //mauiwin.ExtendsContentIntoTitleBar = false;
                    //mauiwin.Title = "QIANSHI CHAT";

                    //var winId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(mauiwin.WindowHandle);
                    //var appwin = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(winId);

                    //var customOverlappedPresenter = Microsoft.UI.Windowing.OverlappedPresenter.CreateForContextMenu();
                    //appwin.SetPresenter(customOverlappedPresenter);
                });
            });
            return builder;
        }
    }
}