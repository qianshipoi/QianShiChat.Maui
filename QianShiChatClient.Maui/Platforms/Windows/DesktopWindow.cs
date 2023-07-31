using Microsoft.Maui.Platform;
using Microsoft.UI;
using Microsoft.UI.Windowing;

using WinRT.Interop;

namespace QianShiChatClient.Maui.Windows
{
    public partial class DesktopWindow
    {
        private Microsoft.UI.Xaml.Window _window;

        public void NoResize()
        {
            var appWindow = _window.GetAppWindow();
            if (appWindow is null)
                return;

            _window.ExtendsContentIntoTitleBar = true;
            //var customOverlappedPresenter = Microsoft.UI.Windowing.OverlappedPresenter.CreateForContextMenu();
            //appWindow.SetPresenter(customOverlappedPresenter);
        }

        protected override void OnCreated()
        {
            var screenWidth = DeviceDisplay.Current.MainDisplayInfo.Width;
            var screenHeight = DeviceDisplay.Current.MainDisplayInfo.Height;

            X = (screenWidth - Width) / 2;
            Y = (screenHeight - Height) / 2;

            if (Handler?.PlatformView is not Microsoft.UI.Xaml.Window _window)
                return;

            var hwnd = WindowNative.GetWindowHandle(_window);
            var id = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(id);
        }
    }
}
