namespace QianShiChatClient.Maui.Views;

public partial class ChatMessageView : ContentView
{
    public ChatMessageView()
    {
        InitializeComponent();
    }

    public ChatMessageView(ChatMessageViewModel viewModel) : base()
    {
        this.BindingContext = viewModel;
    }
}