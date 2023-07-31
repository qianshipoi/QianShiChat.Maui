namespace QianShiChatClient.Maui.Views.Desktop;

public partial class DesktopAddQueryPage : ContentPage
{
	public DesktopAddQueryPage(AddQueryViewModel viewModel)
	{
		InitializeComponent();
		this.BindingContext = viewModel;
    }
}