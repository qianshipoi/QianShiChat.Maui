using QianShiChatClient.Application.IServices;

namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class AddQueryViewModel : ViewModelBase
{
    private const string QueryState = "Query";
    private const string AddState = "Add";

    private readonly IApiClient _apiClient;
    private readonly ILogger<AddQueryViewModel> _logger;

    [ObservableProperty]
    private string _searchContent;

    [ObservableProperty]
    private string _currentState;

    public AddFriendViewModel AddFriendVM { get; set; }

    public ObservableCollection<UserInfo> Result { get; }

    public AddQueryViewModel(IApiClient apiClient, ILogger<AddQueryViewModel> logger)
    {
        Result = new ObservableCollection<UserInfo>();
        _apiClient = apiClient;
        _logger = logger;
        CurrentState = QueryState;
    }

    [RelayCommand]
    private async Task Search()
    {
        if (IsBusy || string.IsNullOrWhiteSpace(SearchContent))
            return;

        IsBusy = true;
        try
        {
            var paged = await _apiClient.SearchNickNameAsync(SearchContent);
            Result.Clear();
            foreach (var user in paged.Items)
            {
                var friend = user.ToUserInfo();
                Result.Add(friend);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task AddFriend(UserInfo user)
    {
        if (MauiAppConsts.IsDesktop)
        {
            AddFriendVM = ServiceHelper.GetService<AddFriendViewModel>();
            AddFriendVM.UserInfo = user;
            CurrentState = AddState;
            return Task.CompletedTask;
        }
        else
        {
            return _navigationService.GoToAddFriendPage(user);
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        CurrentState = QueryState;
    }
}