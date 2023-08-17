namespace QianShiChatClient.Maui.Windows;

public partial class DesktopWindow : Window
{
    private string _winId;

    public DesktopWindow(string winId) : base()
    {
        _winId = winId;
    }

    public DesktopWindow(string winId, Page page) : base(page)
    {
        _winId = winId;
        Width = 500;
        Height = 500;
    }

    protected override void OnDestroying()
    {
        ServiceHelper.GetService<IWindowManagerService>()?.CloseWindow(_winId);
        base.OnDestroying();
    }
}
