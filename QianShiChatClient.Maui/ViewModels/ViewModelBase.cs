namespace QianShiChatClient.Maui.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    protected readonly INavigationService _navigationService;
    protected readonly IStringLocalizer<MyStrings> _stringLocalizer;

    public ViewModelBase()
    {
        _navigationService = ServiceHelper.GetService<INavigationService>();
        _stringLocalizer = ServiceHelper.GetService<IStringLocalizer<MyStrings>>();
    }

    [ObservableProperty]
    protected bool _isBusy;

    [ObservableProperty]
    protected string _title;

    public UserInfo User => App.Current.User;
}