namespace QianShiChatClient.Maui.Views;

public partial class ScanningPage : ContentPage
{
    public ScanningPage(ScanningViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}