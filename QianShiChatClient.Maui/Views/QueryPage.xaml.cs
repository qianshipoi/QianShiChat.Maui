namespace QianShiChatClient.Maui.Views;

public partial class QueryPage : ContentPage
{
    public QueryPage(QueryViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}