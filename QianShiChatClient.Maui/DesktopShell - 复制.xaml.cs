namespace QianShiChatClient.Maui;

public partial class DesktopShell1 : Shell
{
    public DesktopShell1(DesktopShellViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}