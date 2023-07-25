namespace QianShiChatClient.Maui.Views;

public partial class UserInfoView : ContentView
{
    public UserInfoView(UserInfoViewModel viewModel)
	{
		InitializeComponent();
		this.BindingContext = viewModel;

    }
}