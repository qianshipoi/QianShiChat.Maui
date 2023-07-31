#if WINDOWS
using Microsoft.Maui.Platform;

using PInvoke;

using static PInvoke.User32;

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoftui = Microsoft.UI;
using MicrosoftuiWindowing = Microsoft.UI.Windowing;
using MicrosoftuiXaml = Microsoft.UI.Xaml;
using MicrosoftuixmlMedia = Microsoft.UI.Xaml.Media;
using WinRT.Interop;
#endif

namespace QianShiChatClient.Maui.Windows
{
    public class DesktopWindow : Microsoft.Maui.Controls.Window
    {
        public DesktopWindow() : base()
        {
        }

        public DesktopWindow(Page page) : base(page)
        {
            Width = 500;
            Height = 500;
        }

        public void NoResize()
        { 
#if WINDOWS
        if (Handler?.PlatformView is not MicrosoftuiXaml.Window winuiWindow)
            return;
        var appWindow = winuiWindow.GetAppWindow();
        if (appWindow is null)
            return;

        winuiWindow.ExtendsContentIntoTitleBar = true;
        //var customOverlappedPresenter = Microsoft.UI.Windowing.OverlappedPresenter.CreateForContextMenu();
        //appWindow.SetPresenter(customOverlappedPresenter);
#endif
        }

        protected override void OnCreated()
        {
            base.OnCreated();

            var screenWidth = DeviceDisplay.Current.MainDisplayInfo.Width;
            var screenHeight = DeviceDisplay.Current.MainDisplayInfo.Height;

            X = (screenWidth - Width) / 2;
            Y = (screenHeight - Height) / 2;

#if WINDOWS
            if (Handler?.PlatformView is not MicrosoftuiXaml.Window winuiWindow)
                return;

            var hwnd = WindowNative.GetWindowHandle(winuiWindow);
            var id = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(id);
#endif
        }

        protected override void OnDestroying()
        {
            ServiceHelper.GetService<IWindowManagerService>()?.CloseWindow(this);
            base.OnDestroying();
        }
    }
}
