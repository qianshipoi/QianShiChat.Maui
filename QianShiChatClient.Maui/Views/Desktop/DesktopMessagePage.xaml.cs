namespace QianShiChatClient.Maui.Views.Desktop;

public partial class DesktopMessagePage : ContentPage
{
    public DesktopMessagePage(MessageViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }

    private void OpenChatWindow_Clicked(object sender, EventArgs e)
    {
        if (sender is not MenuFlyoutItem item || item.CommandParameter is not Session session)
        {
            return;
        }
        (BindingContext as MessageViewModel).OpenNewWindowCommand.Execute(session);
    }
}