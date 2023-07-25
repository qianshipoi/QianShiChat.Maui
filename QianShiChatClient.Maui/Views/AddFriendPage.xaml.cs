namespace QianShiChatClient.Maui.Views;

public partial class AddFriendPage : ContentPage
{
    public AddFriendPage(AddFriendViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}