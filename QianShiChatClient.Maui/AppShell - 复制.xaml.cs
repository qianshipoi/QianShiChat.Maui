namespace QianShiChatClient.Maui;

public partial class AppShell1 : Shell
{
    public AppShell1(AppShellViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}