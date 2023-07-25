namespace QianShiChatClient.Maui.Views.Desktop;

public partial class DesktopMessagePage : ContentPage
{
    public DesktopMessagePage(MessageViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}