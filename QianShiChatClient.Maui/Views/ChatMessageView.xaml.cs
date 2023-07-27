namespace QianShiChatClient.Maui.Views;

public partial class ChatMessageView : ContentView
{
    public ChatMessageView()
    {
        InitializeComponent();
    }

    public ChatMessageView(ChatMessageViewModel viewModel) : this()
    {
        this.BindingContext = viewModel;
    }
}