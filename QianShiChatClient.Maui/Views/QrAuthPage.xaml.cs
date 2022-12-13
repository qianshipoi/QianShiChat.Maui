namespace QianShiChatClient.Maui.Views;

public partial class QrAuthPage : ContentPage
{
    public QrAuthPage(QrAuthViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}