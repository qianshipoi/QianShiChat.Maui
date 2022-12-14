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

    public async Task<UserDto> LoginAsync(LoginReqiest request, CancellationToken cancellationToken = default)
    {
        using var response = await _client.PostAsJsonAsync("/api/Auth", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var accessToken = response.Headers.GetValues("x-access-token").FirstOrDefault();
        AccessToken = accessToken;
        var body = await response.Content.ReadAsStringAsync();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
        return JsonSerializer.Deserialize<UserDto>(body, _serializerOptions);
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
            var user = JsonSerializer.Deserialize<UserDto>(body, _serializerOptions);
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
        return await _client.GetFromJsonAsync<List<UserWithMessageDto>>("/api/Friend/Unread", _serializerOptions, cancellationToken);
    }

    public async Task<ChatMessageDto> SendTextAsync(PrivateChatMessageRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsJsonAsync($"/api/chat/text", request, _serializerOptions, cancellationToken);
        await HandleUnauthorizedResponse(response);
        return await response.Content.ReadFromJsonAsync<ChatMessageDto>();
    }

    public async Task<PagedList<UserDto>> SearchNickNameAsync(string searchContent, uint page = 1, uint size = 20, CancellationToken cancellationToken = default)
    {
        return await _client.GetFromJsonAsync<PagedList<UserDto>>($"/api/User/{page}/{size}?NickName={searchContent}", _serializerOptions, cancellationToken);
    }

    public async Task FriendApplyAsync(FriendApplyRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsJsonAsync("/api/FriendApply", request, _serializerOptions, cancellationToken);
        await HandleUnauthorizedResponse(response);
    }

    public async Task<PagedList<ApplyPendingDto>> FriendApplyPendingAsync(FriendApplyPendingRequest request, CancellationToken cancellationToken = default)
    {
        return await _client.GetFromJsonAsync<PagedList<ApplyPendingDto>>(FormatParam($"/api/FriendApply/Pending", request), _serializerOptions, cancellationToken);
    }

    public async Task<List<UserDto>> AllFriendAsync(CancellationToken cancellationToken = default)
    {
        return await _client.GetFromJsonAsync<List<UserDto>>("/api/Friend", cancellationToken);
    }

    public string FormatFile(string url) => BaseAddress + url;

    public async Task<QrAuthResponse> QrPreAuthAsync(string key, CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsync($"/api/Auth/qr/preauth?key={key}", null, cancellationToken: cancellationToken);
        await HandleUnauthorizedResponse(response);
        return JsonSerializer.Deserialize<QrAuthResponse>(await response.Content.ReadAsStringAsync(cancellationToken), _serializerOptions);
    }

    public async Task<QrAuthResponse> QrAuthAsync(string key, CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsync($"/api/Auth/qr/auth?key={key}", null, cancellationToken: cancellationToken);
        await HandleUnauthorizedResponse(response);
        return JsonSerializer.Deserialize<QrAuthResponse>(await response.Content.ReadAsStringAsync(cancellationToken), _serializerOptions);
    }

    public async Task<CreateQrAuthKeyResponse> CreateQrKeyAsync(CancellationToken cancellationToken = default)
    {
        var response = await _client.GetAsync("/api/Auth/qr/key", cancellationToken);
        await HandleUnauthorizedResponse(response);
        return JsonSerializer.Deserialize<CreateQrAuthKeyResponse>(await response.Content.ReadAsStringAsync(cancellationToken), _serializerOptions);
    }

    public async Task<CreateQrCodeResponse> CreateQrCodeAsync(CreateQrCodeRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _client.PostAsJsonAsync("/api/Auth/qr/create", request, cancellationToken);
        await HandleUnauthorizedResponse(response);
        return JsonSerializer.Deserialize<CreateQrCodeResponse>(await response.Content.ReadAsStringAsync(cancellationToken), _serializerOptions);
    }

    public async Task<CheckQrAuthKeyResponse> CheckQrKeyAsync(string key,CancellationToken cancellationToken = default)
    {
        var response = await _client.GetAsync($"/api/Auth/qr/check?key={key}", cancellationToken);
        await HandleUnauthorizedResponse(response);
        return JsonSerializer.Deserialize<CheckQrAuthKeyResponse>(await response.Content.ReadAsStringAsync(cancellationToken), _serializerOptions);
    }

    string FormatParam(string url, object obj, bool ignoreNull = true)
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

    async Task HandleUnauthorizedResponse(HttpResponseMessage response)
    {
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await _navigationService.GoToLoginPage();
            return;
        }
        response.EnsureSuccessStatusCode();
    }
}
