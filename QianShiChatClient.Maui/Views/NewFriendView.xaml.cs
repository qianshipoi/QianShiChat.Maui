namespace QianShiChatClient.Maui.Views;

public partial class NewFriendView : ContentView
{
	public NewFriendView(NewFriendViewModel viewModel)
	{
		InitializeComponent();
		this.BindingContext = viewModel;
	}
}