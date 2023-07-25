#if WINDOWS
using Microsoft.Maui.Platform;

using PInvoke;

using static PInvoke.User32;

using Microsoftui = Microsoft.UI;
using MicrosoftuiWindowing = Microsoft.UI.Windowing;
using MicrosoftuiXaml = Microsoft.UI.Xaml;
using MicrosoftuixmlMedia = Microsoft.UI.Xaml.Media;
#endif

namespace QianShiChatClient.Maui.Views.Desktop;

public partial class DesktopPage : ContentPage
{
    public DesktopPage(DesktopShellViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }

    private void Button_Clicked(object sender, EventArgs e)
    {
#if WINDOWS
        if (Window.Handler?.PlatformView is not MicrosoftuiXaml.Window winuiWindow)
            return;

        var windowHanlde = winuiWindow.GetWindowHandle();
        User32.PostMessage(windowHanlde, WindowMessage.WM_SYSCOMMAND, new IntPtr((int)SysCommands.SC_MAXIMIZE), IntPtr.Zero);
#endif
    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {
#if WINDOWS
        if (Window.Handler?.PlatformView is not MicrosoftuiXaml.Window winuiWindow)
            return;

        var windowHanlde = winuiWindow.GetWindowHandle();
        User32.PostMessage(windowHanlde, WindowMessage.WM_SYSCOMMAND, new IntPtr((int)SysCommands.SC_MINIMIZE), IntPtr.Zero);
#endif
    }

    private void Button_Clicked_2(object sender, EventArgs e)
    {
#if WINDOWS

        if (Window.Handler?.PlatformView is not MicrosoftuiXaml.Window winuiWindow)
            return;
        var appWindow = winuiWindow.GetAppWindow();
        if (appWindow is null)
            return;

        var displyArea = MicrosoftuiWindowing.DisplayArea.Primary;
        double scalingFactor = winuiWindow.GetDisplayDensity();
        var width = 800 * scalingFactor;
        var height = 600 * scalingFactor;
        double startX = (displyArea.WorkArea.Width - width) / 2.0;
        double startY = (displyArea.WorkArea.Height - height) / 2.0;

        appWindow.MoveAndResize(new((int)startX, (int)startY, (int)width, (int)height), displyArea);

        //appWindow.TitleBar?.ResetToDefault();
#endif
    }

    private void Button_Clicked_3(object sender, EventArgs e)
    {
#if WINDOWS
        if (Window.Handler?.PlatformView is not MicrosoftuiXaml.Window winuiWindow)
            return;
        var appWindow = winuiWindow.GetAppWindow();
        if (appWindow is null)
            return;

        //注意由于Maui默认开启了扩展TitleBar(标题栏融合模式？)所以先要去掉 否则全屏仍然会出现 关闭按钮
        //虽然关闭了标题栏融合模式，但是全屏时仍然会存在一个类似标题栏的东西，如果需要处理需要进行深度定制（可以查看我的github项目）
        winuiWindow.Title = "MyTestApp";
        winuiWindow.ExtendsContentIntoTitleBar = false;
        appWindow.SetPresenter(MicrosoftuiWindowing.AppWindowPresenterKind.FullScreen);
#endif
    }

    private void Button_Clicked_4(object sender, EventArgs e)
    {
#if WINDOWS
        if (Window.Handler?.PlatformView is not MicrosoftuiXaml.Window winuiWindow)
            return;
        var appWindow = winuiWindow.GetAppWindow();
        if (appWindow is null)
            return;

        winuiWindow.ExtendsContentIntoTitleBar = true;
        appWindow.SetPresenter(MicrosoftuiWindowing.AppWindowPresenterKind.Default);

        var application = MicrosoftuiXaml.Application.Current;
        var res = application.Resources;
        res["WindowCaptionBackground"] = res["WindowCaptionBackgroundDisabled"];

        //修改标题栏后需要主动刷新才会生效
        TriggertTitleBarRepaint();

#endif
    }

    private void Button_Clicked_5(object sender, EventArgs e)
    {
#if WINDOWS
        if (Window.Handler?.PlatformView is not MicrosoftuiXaml.Window winuiWindow)
            return;
        var appWindow = winuiWindow.GetAppWindow();
        if (appWindow is null)
            return;

        var customOverlappedPresenter = Microsoft.UI.Windowing.OverlappedPresenter.Create();
        appWindow.SetPresenter(customOverlappedPresenter);
        winuiWindow.ExtendsContentIntoTitleBar = false;
#endif
    }

    private void Button_Clicked_6(object sender, EventArgs e)
    {
#if WINDOWS
        if (Window.Handler?.PlatformView is not MicrosoftuiXaml.Window winuiWindow)
            return;
        var appWindow = winuiWindow.GetAppWindow();
        if (appWindow is null)
            return;

        winuiWindow.ExtendsContentIntoTitleBar = false;
        var customOverlappedPresenter = Microsoft.UI.Windowing.OverlappedPresenter.CreateForContextMenu();
        appWindow.SetPresenter(customOverlappedPresenter);
#endif
    }

    private void Button_Clicked_7(object sender, EventArgs e)
    {
#if WINDOWS
        if (Window.Handler?.PlatformView is not Microsoft.UI.Xaml.Window winuiWindow)
            return;

        var application = Microsoft.UI.Xaml.Application.Current;
        var res = application.Resources;

        //看到这里你一定会疑惑为什么是这样，如果你有兴趣，可以查阅Winui3的源码
        res["WindowCaptionBackground"] = new MicrosoftuixmlMedia.SolidColorBrush(Microsoft.UI.Colors.Red);

        //修改标题栏后需要主动刷新才会生效（否则需要你人为进行一次最小化处理）
        TriggertTitleBarRepaint();
#endif
    }

    private bool TriggertTitleBarRepaint()
    {
#if WINDOWS
        if (Window.Handler?.PlatformView is not MicrosoftuiXaml.Window winuiWindow)
            return false;

        var windowHanlde = winuiWindow.GetWindowHandle();
        var activeWindow = User32.GetActiveWindow();
        if (windowHanlde == activeWindow)
        {
            User32.PostMessage(windowHanlde, WindowMessage.WM_ACTIVATE, new IntPtr((int)0x00), IntPtr.Zero);
            User32.PostMessage(windowHanlde, WindowMessage.WM_ACTIVATE, new IntPtr((int)0x01), IntPtr.Zero);
        }
        else
        {
            User32.PostMessage(windowHanlde, WindowMessage.WM_ACTIVATE, new IntPtr((int)0x01), IntPtr.Zero);
            User32.PostMessage(windowHanlde, WindowMessage.WM_ACTIVATE, new IntPtr((int)0x00), IntPtr.Zero);
        }

#endif
        return true;
    }
}