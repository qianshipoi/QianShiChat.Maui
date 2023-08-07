using QianShiChatClient.MauiBlazor.ViewModels;
namespace QianShiChatClient.MauiBlazor.Views;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel loginViewModel)
    {
        InitializeComponent();
        this.BindingContext = loginViewModel;
    }
}