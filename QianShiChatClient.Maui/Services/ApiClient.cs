using System.Web;

namespace QianShiChatClient.Maui.Services;

public class ApiClient : IApiClient
{
    readonly HttpClient _client;
    readonly INavigationService _navigationService;
    JsonSerializerOptions _serializerOptions;

    public static string BaseAddress =
        DeviceInfo.Platform == DevicePlatform.Android
        ? "https://chat.kuriyama.top"
        : "https://chat.kuriyama.top";

    public static string CurrentClientType = $"MauiClient{DeviceInfo.Platform}{DeviceInfo.Idiom}";

    public ApiClient(HttpClient client, INavigationService navigationService)
    {
        _navigationService = navigationService;
        _client = client;
        _client.BaseAddress = new Uri(BaseAddress);
        _client.DefaultRequestHeaders.Add("Accept", "application/json");
        _client.DefaultRequestHeaders.Add("User-Agent", "QianShiChatClient-Maui");
        _client.DefaultRequestHeaders.Add("Client-Type", ClientType);
        if (!string.IsNullOrEmpty(AccessToken))
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
        }
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public string AccessToken
    {
        get => Settings.AccessToken;
        private set => Settings.AccessToken = value;
    }

    public string ClientType => CurrentClientType;

    public async Task<UserDto> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await _client.PostAsJsonAsync("/api/Auth", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var accessToken = response.Headers.GetValues("x-access-token").FirstOrDefault();
        AccessToken = accessToken;
        var body = await response.Content.ReadAsStringAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
        var result = JsonSerializer.Deserialize<GlobalResult<UserDto>>(body, _serializerOptions);
        return result.Data;
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
            var user = JsonSerializer.Deserialize<GlobalResult<UserDto>>(body, _serializerOptions);
            return (true, user.Data);
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show(); ;
            return (false, null);
        }
    }

    public async Task<List<UserWithMessageDto>> GetUnreadMessageFriendsAsync(CancellationToken cancellationToken = default)
    {
        var result = await _client.GetFromJsonAsync<GlobalResult<List<UserWithMessageDto>>>("/api/Friend/Unread", _serializerOptions, cancellationToken);
        return result.Data;
    }

    public async Task<ChatMessageDto> SendTextAsync(PrivateChatMessageRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsJsonAsync($"/api/chat/text", request, _serializerOptions, cancellationToken);
        await HandleUnauthorizedResponse(response);
        var result = await response.Content.ReadFromJsonAsync<GlobalResult<ChatMessageDto>>();
        return result.Data;
    }

    public async Task<PagedList<UserDto>> SearchNickNameAsync(string searchContent, uint page = 1, uint size = 20, CancellationToken cancellationToken = default)
    {
        var result = await _client.GetFromJsonAsync<GlobalResult<PagedList<UserDto>>>($"/api/User/{page}/{size}?NickName={searchContent}", _serializerOptions, cancellationToken);
        return result.Data;
    }

    public async Task FriendApplyAsync(FriendApplyRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsJsonAsync("/api/FriendApply", request, _serializerOptions, cancellationToken);
        await HandleUnauthorizedResponse(response);
    }

    public async Task<PagedList<ApplyPendingDto>> FriendApplyPendingAsync(FriendApplyPendingRequest request, CancellationToken cancellationToken = default)
    {
        var result = await _client.GetFromJsonAsync<GlobalResult<PagedList<ApplyPendingDto>>>(FormatParam($"/api/FriendApply/Pending", request), _serializerOptions, cancellationToken);
        return result.Data;
    }

    public async Task<List<UserDto>> AllFriendAsync(CancellationToken cancellationToken = default)
    {
        var result = await _client.GetFromJsonAsync<GlobalResult<List<UserDto>>>("/api/Friend", cancellationToken);
        return result.Data;
    }

    public string FormatFile(string url) => BaseAddress + url;

    public async Task<QrAuthResponse> QrPreAuthAsync(string key, CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsync($"/api/Auth/qr/preauth?key={key}", null, cancellationToken: cancellationToken);
        await HandleUnauthorizedResponse(response);
        var result = JsonSerializer.Deserialize<GlobalResult<QrAuthResponse>>(await response.Content.ReadAsStringAsync(cancellationToken), _serializerOptions);
        return result.Data;
    }

    public async Task<QrAuthResponse> QrAuthAsync(string key, CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsync($"/api/Auth/qr/auth?key={key}", null, cancellationToken: cancellationToken);
        await HandleUnauthorizedResponse(response);
        var result = JsonSerializer.Deserialize<GlobalResult<QrAuthResponse>>(await response.Content.ReadAsStringAsync(cancellationToken), _serializerOptions);
        return result.Data;
    }

    public async Task<CreateQrAuthKeyResponse> CreateQrKeyAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetAsync("/api/Auth/qr/key", cancellationToken);
        await HandleUnauthorizedResponse(response);
        var result = JsonSerializer.Deserialize<GlobalResult<CreateQrAuthKeyResponse>>(await response.Content.ReadAsStringAsync(cancellationToken), _serializerOptions);
        return result.Data;
    }

    public async Task<CreateQrCodeResponse> CreateQrCodeAsync(CreateQrCodeRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsJsonAsync("/api/Auth/qr/create", request, cancellationToken);
        await HandleUnauthorizedResponse(response);
        var result = JsonSerializer.Deserialize<GlobalResult<CreateQrCodeResponse>>(await response.Content.ReadAsStringAsync(cancellationToken), _serializerOptions);
        return result.Data;
    }

    public async Task<CheckQrAuthKeyResponse> CheckQrKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        var response = await _client.GetAsync($"/api/Auth/qr/check?key={key}", cancellationToken);
        await HandleUnauthorizedResponse(response);
        var result = JsonSerializer.Deserialize<GlobalResult<CheckQrAuthKeyResponse>>(await response.Content.ReadAsStringAsync(cancellationToken), _serializerOptions);
        return result.Data;
    }

    private string FormatParam(string url, object obj, bool ignoreNull = true)
    {
        var properties = obj.GetType().GetProperties();
        var sb = new StringBuilder();
        sb.Append(url);
        sb.Append('?');
        foreach (var property in properties)
        {
            var v = property.GetValue(obj, null);
            if (v == null)
                continue;

            sb.Append(property.Name);
            sb.Append("=");
            sb.Append(HttpUtility.UrlEncode(v.ToString()));
            sb.Append("&");
        }
        sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }

    private async Task HandleUnauthorizedResponse(HttpResponseMessage response)
    {
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await _navigationService.GoToLoginPage();
            return;
        }
        response.EnsureSuccessStatusCode();
    }

    public async Task<UserDto> FindUserAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _client.GetAsync($"/api/users/{id}", cancellationToken);
        await HandleUnauthorizedResponse(response);
        var result = JsonSerializer.Deserialize<GlobalResult<UserDto>>(await response.Content.ReadAsStringAsync(cancellationToken), _serializerOptions);
        return result.Data;
    }
}
