namespace QianShiChatClient.Maui;

public partial class AppShell : Shell
{
    readonly ChatHub _chatHub;

    public AppShell(ChatHub chatHub)
    {
        InitializeComponent();
        _chatHub = chatHub;

    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await _chatHub.Connect();
    }
}
