namespace QianShiChatClient.Maui.Views;

public partial class SettingsPage : ContentPage
{
	public SettingsPage(SettingsViewModel viewModel)
	{
		InitializeComponent();
		this.BindingContext = viewModel;
	}
}