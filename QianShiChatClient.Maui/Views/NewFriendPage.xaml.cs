namespace QianShiChatClient.Maui.Views;

public partial class NewFriendPage : ContentPage
{
    public NewFriendPage(NewFriendViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}