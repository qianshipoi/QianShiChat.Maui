namespace QianShiChatClient.Maui.Services;

public class ApiClient : IApiClient
{
    readonly HttpClient _client;
    readonly INavigationService _navigationService;

    public static string BaseAddress =
        DeviceInfo.Platform == DevicePlatform.Android
        ? "https://chat.kuriyama.top"
        : "https://chat.kuriyama.top";

    public ApiClient(HttpClient client, INavigationService navigationService)
    {
        _navigationService = navigationService;
        _client = client;
        _client.BaseAddress = new Uri(BaseAddress);
        _client.DefaultRequestHeaders.Add("Accept", "application/json");
        _client.DefaultRequestHeaders.Add("User-Agent", "QianShiChatClient-Maui");
        if (!string.IsNullOrEmpty(AccessToken))
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
        }
    }

    public string AccessToken
    {
        get => Settings.AccessToken;
        private set => Settings.AccessToken = value;
    }

    public async Task<UserDto> LoginAsync(LoginReqiest request, CancellationToken cancellationToken = default)
    {
        using var response = await _client.PostAsJsonAsync("/api/Auth", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var accessToken = response.Headers.GetValues("x-access-token").FirstOrDefault();
        AccessToken = accessToken;
        var body = await response.Content.ReadAsStringAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
        return JsonSerializer.Deserialize<UserDto>(body);
    }

    public async Task<(bool, UserDto)> CheckAccessToken(string token, CancellationToken cancellationToken = default)
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        try
        {
            using var response = await _client.GetAsync("/api/auth", cancellationToken);
            await HandleUnauthorizedResponse(response);
            var accessToken = response.Headers.GetValues("x-access-token").FirstOrDefault();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            AccessToken = accessToken;
            var body = await response.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<UserDto>(body);
            return (true, user);
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show(); ;
            return (false, null);
        }
    }

    public async Task<List<UserWithMessageDto>> GetUnreadMessageFriendsAsync(CancellationToken cancellationToken = default)
    {
        return await _client.GetFromJsonAsync<List<UserWithMessageDto>>("/api/Friend/Unread", cancellationToken);
    }

    public async Task<ChatMessageDto> SendTextAsync(PrivateChatMessageRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsJsonAsync($"/api/chat/text", request, cancellationToken);
        await HandleUnauthorizedResponse(response);
        return await response.Content.ReadFromJsonAsync<ChatMessageDto>();
    }

    public async Task<PagedList<UserDto>> SearchNickNameAsync(string searchContent, uint page = 1, uint size = 20, CancellationToken cancellationToken = default)
    {
        return await _client.GetFromJsonAsync<PagedList<UserDto>>($"/api/User/{page}/{size}?NickName={searchContent}", cancellationToken);
    }

    public async Task FriendApplyAsync(FriendApplyRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsJsonAsync("/api/FriendApply", request, cancellationToken);
        await HandleUnauthorizedResponse(response);
    }

    async Task HandleUnauthorizedResponse(HttpResponseMessage response)
    {
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await _navigationService.GoToLoginPage();
            return;
        }
        response.EnsureSuccessStatusCode();
    }

    public string FormatFile(string url) => BaseAddress + url;
}
