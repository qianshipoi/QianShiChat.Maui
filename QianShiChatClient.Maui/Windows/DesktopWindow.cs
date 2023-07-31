namespace QianShiChatClient.Maui.Windows;

public partial class DesktopWindow : Window
{
    public DesktopWindow() : base()
    {
    }

    public DesktopWindow(Page page) : base(page)
    {
        Width = 500;
        Height = 500;
    }

    protected override void OnDestroying()
    {
        ServiceHelper.GetService<IWindowManagerService>()?.CloseWindow(this);
        base.OnDestroying();
    }
}
