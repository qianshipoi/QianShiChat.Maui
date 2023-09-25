using QianShiChatClient.Application.IServices;

namespace QianShiChatClient.MauiBlazor.ViewModels;

public partial class ViewModelBase : ObservableObject
{
    protected readonly INavigationService _navigationService;

    public ViewModelBase()
    {
        _navigationService = ServiceHelper.GetService<INavigationService>();
    }

    [ObservableProperty]
    protected bool _isBusy;

    [ObservableProperty]
    protected string _title;

    public UserInfoModel User => App.Current.User;
}
