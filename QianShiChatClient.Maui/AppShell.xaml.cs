namespace QianShiChatClient.Maui;

public partial class AppShell : Shell
{
    readonly ChatHub _chatHub;

    public AppShell(AppShellViewModel viewModel, ChatHub chatHub)
    {
        InitializeComponent();
        _chatHub = chatHub;
        this.BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _chatHub.Connect();
    }
}
