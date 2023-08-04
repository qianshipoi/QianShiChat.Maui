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
            if (appWindow.Presenter is OverlappedPresenter p)
            {
                p.IsResizable = false;
                p.IsMaximizable = false;
                p.IsMinimizable = false;
            }
            //_window.ExtendsContentIntoTitleBar = true;
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

            this._window = _window;

            var hwnd = WindowNative.GetWindowHandle(_window);
            var id = Win32Interop.GetWindowIdFromWindow(hwnd);
            var appWindow = AppWindow.GetFromWindowId(id);
        }
    }
}
