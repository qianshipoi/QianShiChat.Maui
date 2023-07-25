namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class QueryViewModel : ViewModelBase
{
    private readonly IApiClient _apiClient;

    [ObservableProperty]
    private string _searchContent;

    public ObservableCollection<UserInfo> Result { get; }

    public QueryViewModel(
        IStringLocalizer<MyStrings> stringLocalizer,
        INavigationService navigationService,
        IApiClient apiClient)
        : base(navigationService, stringLocalizer)
    {
        Result = new ObservableCollection<UserInfo>();
        _apiClient = apiClient;
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
                //friend.Avatar = _apiClient.FormatFile(friend.Avatar);
                Result.Add(friend);
            }
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private Task AddFriend(UserInfo user) => _navigationService.GoToAddFriendPage(user);
}