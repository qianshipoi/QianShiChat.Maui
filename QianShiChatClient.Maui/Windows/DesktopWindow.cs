#if WINDOWS
using Microsoft.Maui.Platform;

using PInvoke;

using static PInvoke.User32;

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
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

        protected override void OnCreated()
        {
            base.OnCreated();

            var screenWidth = DeviceDisplay.Current.MainDisplayInfo.Width;
            var screenHeight = DeviceDisplay.Current.MainDisplayInfo.Height;

            X = (screenWidth - Width) / 2;
            Y = (screenHeight - Height) / 2;

#if WINDOWS
            if (Handler?.PlatformView is not Microsoft.UI.Xaml.Window winuiWindow)
                return;
            var hwnd = WindowNative.GetWindowHandle(winuiWindow);
            WindowId id = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(id);
#endif
        }

        protected override void OnDestroying()
        {
            ServiceHelper.GetService<WindowManagerService>()?.CloseWindow(this);
            base.OnDestroying();
        }
    }
}
