namespace QianShiChatClient.Maui.Views;

public partial class MessagePage : ContentPage
{
    public MessagePage(MessageViewModel messageViewModel)
    {
        InitializeComponent();
        this.BindingContext = messageViewModel;
    }
}