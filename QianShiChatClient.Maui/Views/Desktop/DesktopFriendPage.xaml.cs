namespace QianShiChatClient.Maui.Views.Desktop;

public partial class DesktopFriendPage : ContentPage
{
    public DesktopFriendPage(FriendViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}