namespace QianShiChatClient.Maui.Views;

public partial class NewFriendDetailPage : ContentPage
{
    public NewFriendDetailPage(NewFriendDetailViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}