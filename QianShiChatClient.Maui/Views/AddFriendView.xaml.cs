namespace QianShiChatClient.Maui.Views;

public partial class AddFriendView : ContentView
{
    public static BindableProperty CancelCommandProperty
         = BindableProperty.Create(nameof(CancelCommand), typeof(ICommand), typeof(AddFriendView));

    public static BindableProperty ShowCancelProperty
         = BindableProperty.Create(nameof(ShowCancel), typeof(bool), typeof(AddFriendView), defaultValue: false);

    public bool ShowCancel
    {
        get => (bool)GetValue(ShowCancelProperty);
        set => SetValue(ShowCancelProperty, value);
    }

    public ICommand CancelCommand
    {
        get => (ICommand)GetValue(CancelCommandProperty);
        set => SetValue(CancelCommandProperty, value);
    }

    public AddFriendView()
    {
        InitializeComponent();
    }
}