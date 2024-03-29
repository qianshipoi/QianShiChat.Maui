﻿namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class SearchViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _searchContent;

    public ObservableCollection<UserInfo> Result { get; }

    public SearchViewModel()
    {
        Result = new ObservableCollection<UserInfo>();
    }

    [RelayCommand]
    private async Task Search()
    {
        if (IsBusy || string.IsNullOrWhiteSpace(SearchContent))
            return;

        IsBusy = true;
        try
        {
            await Task.Delay(2000);
            //var friends = FakerFriend.GetFriends(20);
            //foreach (var friend in friends)
            //{
            //    friend.Avatar = "default_avatar.png";
            //    Result.Add(friend);
            //}
        }
        finally
        {
            IsBusy = false;
        }
    }
}