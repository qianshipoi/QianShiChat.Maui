namespace QianShiChatClient.Maui.Views;

public partial class MessageDetailPage : ContentPage
{
    public MessageDetailPage(MessageDetailViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await Task.Delay(200);
        await MessageControl.ScrollToAsync(MessageCantainer, ScrollToPosition.End, false);
    }
}