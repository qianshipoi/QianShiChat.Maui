namespace QianShiChatClient.Maui.Views;

public partial class SearchPage : ContentPage
{
    public SearchPage(SearchViewModel voiewModel)
    {
        InitializeComponent();
        this.BindingContext = voiewModel;
    }
}