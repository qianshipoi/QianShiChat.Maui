namespace QianShiChatClient.Maui.Services;

public interface IApiClient
{
    string AccessToken { get; }
    string ClientType { get; }

    Task<(bool succeeded, UserDto data, string message)> LoginAsync(LoginReqiest request, CancellationToken cancellationToken = default);
    Task<(bool, UserDto)> CheckAccessToken(string token, CancellationToken cancellationToken = default);
    Task<List<UserWithMessageDto>> GetUnreadMessageFriendsAsync(CancellationToken cancellationToken = default);
    Task<ChatMessageDto> SendTextAsync(PrivateChatMessageRequest request, CancellationToken cancellationToken = default);
    Task<PagedList<UserDto>> SearchNickNameAsync(string searchContent, uint page = 1, uint size = 20, CancellationToken cancellationToken = default);

    Task<UserDto> FindUserAsync(int id, CancellationToken cancellationToken = default);

    Task FriendApplyAsync(FriendApplyRequest request, CancellationToken cancellationToken = default);
    Task<PagedList<ApplyPendingDto>> FriendApplyPendingAsync(FriendApplyPendingRequest request, CancellationToken cancellationToken = default);
    Task<List<UserDto>> AllFriendAsync(CancellationToken cancellationToken = default);
    Task<QrAuthResponse> QrPreAuthAsync(string key, CancellationToken cancellationToken = default);
    Task<QrAuthResponse> QrAuthAsync(string key, CancellationToken cancellationToken = default);
    Task<CreateQrAuthKeyResponse> CreateQrKeyAsync(CancellationToken cancellationToken = default);
    Task<CreateQrCodeResponse> CreateQrCodeAsync(CreateQrCodeRequest request, CancellationToken cancellationToken = default);
    Task<CheckQrAuthKeyResponse> CheckQrKeyAsync(string key, CancellationToken cancellationToken = default);
}
