namespace QianShiChatClient.Maui;

public partial class DesktopShell : Shell
{
    public DesktopShell(DesktopShellViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}