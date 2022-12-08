namespace QianShiChatClient.Maui.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    protected readonly INavigationService _navigationService;
    protected readonly IStringLocalizer<MyStrings> _stringLocalizer;

    public ViewModelBase(
        INavigationService navigationService,
        IStringLocalizer<MyStrings> stringLocalizer)
    {
        _navigationService = navigationService;
        _stringLocalizer = stringLocalizer;
    }

    [ObservableProperty]
    protected bool _isBusy;

    [ObservableProperty]
    protected string _title;

    public UserInfo User => App.Current.User;
}
