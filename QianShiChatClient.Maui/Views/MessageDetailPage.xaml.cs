namespace QianShiChatClient.Maui.Views;

public partial class MessageDetailPage : ContentPage
{
	public MessageDetailPage(MessageDetailViewModel viewModel)
	{
		InitializeComponent();
		this.BindingContext = viewModel;
	}
}