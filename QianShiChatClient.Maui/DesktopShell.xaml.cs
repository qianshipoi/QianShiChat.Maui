namespace QianShiChatClient.Maui;

public partial class DesktopShell : SimpleShell
{
    public DesktopShell(DesktopShellViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }

    private async void ItemClicked(object sender, EventArgs e)
    {
        var button = sender as ContentButton;
        var shellItem = button.BindingContext as BaseShellItem;

        if (!CurrentState.Location.OriginalString.Contains(shellItem.Route))
            await this.GoToAsync($"///{shellItem.Route}", true);
    }
}