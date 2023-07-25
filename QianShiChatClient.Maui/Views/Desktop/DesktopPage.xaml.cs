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

        //ע������MauiĬ�Ͽ�������չTitleBar(�������ں�ģʽ��)������Ҫȥ�� ����ȫ����Ȼ����� �رհ�ť
        //��Ȼ�ر��˱������ں�ģʽ������ȫ��ʱ��Ȼ�����һ�����Ʊ������Ķ����������Ҫ������Ҫ������ȶ��ƣ����Բ鿴�ҵ�github��Ŀ��
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

        //�޸ı���������Ҫ����ˢ�²Ż���Ч
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

        //����������һ�����ɻ�Ϊʲô�����������������Ȥ�����Բ���Winui3��Դ��
        res["WindowCaptionBackground"] = new MicrosoftuixmlMedia.SolidColorBrush(Microsoft.UI.Colors.Red);

        //�޸ı���������Ҫ����ˢ�²Ż���Ч��������Ҫ����Ϊ����һ����С������
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