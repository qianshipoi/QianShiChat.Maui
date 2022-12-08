namespace QianShiChatClient.Maui.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel loginViewModel)
	{
		InitializeComponent();
		this.BindingContext = loginViewModel;
	}
}