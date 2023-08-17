namespace QianShiChatClient.Maui.Views.Desktop;

public partial class DesktopFriendPage : ContentPage
{
    public DesktopFriendPage(FriendViewModel viewModel)
    {
        InitializeComponent();
        this.BindingContext = viewModel;
    }

    private void OpenChatWindow_Clicked(object sender, EventArgs e)
    {
        if(sender is not MenuFlyoutItem item || item.CommandParameter is not UserInfoModel user)
        {
            return;
        }
        (BindingContext as FriendViewModel).OpenNewWindowCommand.Execute(user);
    }
}