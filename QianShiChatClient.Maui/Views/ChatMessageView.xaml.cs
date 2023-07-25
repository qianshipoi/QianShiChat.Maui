namespace QianShiChatClient.Maui.Views;

public partial class ChatMessageView : ContentView
{
    public ChatMessageView(ChatMessageViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }
}