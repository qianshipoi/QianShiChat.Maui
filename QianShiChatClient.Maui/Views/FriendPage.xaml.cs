namespace QianShiChatClient.Maui.Views;

public partial class FriendPage : ContentPage
{
	public FriendPage(FriendViewModel viewModel)
	{
		InitializeComponent();
		this.BindingContext = viewModel;
	}
}