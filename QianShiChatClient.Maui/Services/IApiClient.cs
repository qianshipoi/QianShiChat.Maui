namespace QianShiChatClient.Maui.Services;

public interface IApiClient
{
    string AccessToken { get; }

    Task<UserDto> LoginAsync(LoginReqiest request, CancellationToken cancellationToken = default);

    Task<(bool, UserDto)> CheckAccessToken(string token, CancellationToken cancellationToken = default);

    Task<List<UserWithMessageDto>> GetUnreadMessageFriendsAsync(CancellationToken cancellationToken = default);

    Task<ChatMessageDto> SendTextAsync(PrivateChatMessageRequest request, CancellationToken cancellationToken = default);

    Task<PagedList<UserDto>> SearchNickNameAsync(string searchContent, uint page = 1, uint size = 20, CancellationToken cancellationToken = default);

    Task FriendApplyAsync(FriendApplyRequest request, CancellationToken cancellationToken = default);

    string FormatFile(string url);

    Task<PagedList<ApplyPendingDto>> FriendApplyPendingAsync(FriendApplyPendingRequest request, CancellationToken cancellationToken = default);
    Task<List<UserDto>> AllFriendAsync(CancellationToken cancellationToken = default);
}
