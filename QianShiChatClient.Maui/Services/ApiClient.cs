using System.Diagnostics.Contracts;
using System.Net;

namespace QianShiChatClient.Maui.Services;

public class ApiClient : IApiClient
{
    private const string ACCESS_TOKEN_HEADER_KEY = "x-access-token";

    private readonly INavigationService _navigationService;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ApiClient> _logger;

    public ApiClient(INavigationService navigationService, IHttpClientFactory httpClientFactory, ILogger<ApiClient> logger)
    {
        _navigationService = navigationService;
        _httpClientFactory = httpClientFactory;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        _logger = logger;
    }

    public string AccessToken
    {
        get => Settings.AccessToken;
        private set => Settings.AccessToken = value;
    }

    public string ClientType => AppConsts.CLIENT_TYPE;

    public async Task<(bool succeeded, UserDto data, string message)> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        using var client = _httpClientFactory.CreateClient(AppConsts.API_CLIENT_NAME);
        using var response = await client.PostAsJsonAsync("/api/Auth", request, cancellationToken);
        response.EnsureSuccessStatusCode();
        var accessToken = response.Headers.GetValues(ACCESS_TOKEN_HEADER_KEY).FirstOrDefault();
        AccessToken = accessToken;
        var body = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogInformation("login: request: {request}, response: {response}", JsonSerializer.Serialize(request), body);
        var result = JsonSerializer.Deserialize<GlobalResult<UserDto>>(body, _serializerOptions);
        return (response.IsSuccessStatusCode, result.Data, result.Errors is string str ? str : JsonSerializer.Serialize(result.Errors));
    }

    public async Task<(bool, UserDto)> CheckAccessToken(CancellationToken cancellationToken = default)
    {
        using var client = _httpClientFactory.CreateClient(AppConsts.API_CLIENT_NAME);
        try
        {
            using var response = await client.GetAsync("/api/auth", cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                await _navigationService.GoToLoginPage();
                return (false, null);
            }
            response.EnsureSuccessStatusCode();
            var accessToken = response.Headers.GetValues(ACCESS_TOKEN_HEADER_KEY).FirstOrDefault();
            AccessToken = accessToken;
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            var user = JsonSerializer.Deserialize<GlobalResult<UserDto>>(body, _serializerOptions);
            return (true, user.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "check token error.");
            //await Toast.Make(ex.Message).Show();
            return (false, null);
        }
    }

    private async Task<(bool Succeeded, T Data, string Message)> GetJsonAsync<T>(string url, CancellationToken cancellationToken = default)
    {
        using var client = _httpClientFactory.CreateClient(AppConsts.API_CLIENT_NAME);
        using var response = await client.GetAsync(url, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await _navigationService.GoToLoginPage();
            return (false, default, "Unauthorized！");
        }
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogInformation("http get: {url} - response content: {response}", url, content);

        if (string.IsNullOrWhiteSpace(content))
        {
            return (false, default, "Response is empty.");
        }

        var result = JsonSerializer.Deserialize<GlobalResult<T>>(content, _serializerOptions);

        if (!result.Succeeded)
        {
            return (false, result.Data, result.Errors is string err ? err : JsonSerializer.Serialize(result.Errors, _serializerOptions));
        }

        return (true, result.Data, "Succeeded!");
    }

    private async Task<(bool Succeeded, TResponse Data, string Message)> PostJsonAsync<TResponse, TRequest>(string url, TRequest request, CancellationToken cancellationToken = default)
    {
        using var client = _httpClientFactory.CreateClient(AppConsts.API_CLIENT_NAME);
        using var response = await client.PostAsJsonAsync(url, request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await _navigationService.GoToLoginPage();
            return (false, default, "Unauthorized！");
        }
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogInformation("http post: {url} - response content: {content}", url, content);

        if (string.IsNullOrWhiteSpace(content))
        {
            return (false, default, "Response is empty.");
        }

        var result = JsonSerializer.Deserialize<GlobalResult<TResponse>>(content, _serializerOptions);

        if (!result.Succeeded)
        {
            return (false, result.Data, result.Errors is string err ? err : JsonSerializer.Serialize(result.Errors, _serializerOptions));
        }

        return (true, result.Data, "Succeeded!");
    }

    private async Task<(bool Succeeded, string Message)> PostJsonAsync<TRequest>(string url, TRequest request, CancellationToken
        cancellationToken = default)
    {
        using var client = _httpClientFactory.CreateClient(AppConsts.API_CLIENT_NAME);
        using var response = await client.PostAsJsonAsync(url, request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await _navigationService.GoToLoginPage();
            return (false, "Unauthorized！");
        }
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogInformation("http post: {url} - response content: {response}", url, content);

        if (string.IsNullOrWhiteSpace(content))
        {
            return (false, "Response is empty.");
        }

        var result = JsonSerializer.Deserialize<GlobalResult<object>>(content, _serializerOptions);

        if (!result.Succeeded)
        {
            return (false, result.Errors is string err ? err : JsonSerializer.Serialize(result.Errors, _serializerOptions));
        }

        return (true, "Succeeded!");
    }

    private async Task<(bool Succeeded, TReponse response, string Message)> PostAsync<TReponse>(string url, HttpContent request = null, CancellationToken
       cancellationToken = default)
    {
        using var client = _httpClientFactory.CreateClient(AppConsts.API_CLIENT_NAME);

        using var response = await client.PostAsync(url, request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await _navigationService.GoToLoginPage();
            return (false, default, "Unauthorized！");
        }
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogInformation("http post: {url} - response content: {response}", url, content);

        if (string.IsNullOrWhiteSpace(content))
        {
            return (false, default, "Response is empty.");
        }

        var result = JsonSerializer.Deserialize<GlobalResult<TReponse>>(content, _serializerOptions);

        if (!result.Succeeded)
        {
            return (false, default, result.Errors is string err ? err : JsonSerializer.Serialize(result.Errors, _serializerOptions));
        }

        return (true, result.Data, "Succeeded!");
    }

    private async Task<(bool Succeeded, TReponse response, string Message)> PostProgressAsync<TReponse>(string url, HttpContent request = null, CancellationToken
       cancellationToken = default)
    {
        using var client = _httpClientFactory.CreateClient(AppConsts.API_CLIENT_NAME);

        using var response = await client.PostAsync(url, request, cancellationToken);
        if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
        {
            await _navigationService.GoToLoginPage();
            return (false, default, "Unauthorized！");
        }
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        _logger.LogInformation("http post: {url} - response content: {response}", url, content);

        if (string.IsNullOrWhiteSpace(content))
        {
            return (false, default, "Response is empty.");
        }

        var result = JsonSerializer.Deserialize<GlobalResult<TReponse>>(content, _serializerOptions);

        if (!result.Succeeded)
        {
            return (false, default, result.Errors is string err ? err : JsonSerializer.Serialize(result.Errors, _serializerOptions));
        }

        return (true, result.Data, "Succeeded!");
    }


    public async Task<List<UserWithMessageDto>> GetUnreadMessageFriendsAsync(CancellationToken cancellationToken = default)
    {
        var (succeeded, data, message) = await GetJsonAsync<List<UserWithMessageDto>>("/api/Friend/Unread", cancellationToken);
        return data;
    }

    public async Task<ChatMessageDto> SendTextAsync(PrivateChatMessageRequest request, CancellationToken cancellationToken = default)
    {
        var (succeeded, data, message) = await PostJsonAsync<ChatMessageDto, PrivateChatMessageRequest>("/api/chat/text", request, cancellationToken);
        return data;
    }

    public async Task<ChatMessageDto> SendFileAsync(
        int toId,
        ChatMessageSendType chatMessageSendType,
        string filePath,
        Action<double, double> uploadProgressValue = null,
        CancellationToken cancellationToken = default)
    {
        using var fileStream = File.OpenRead(filePath);
        var size = fileStream.Length;

        var progress = new Progress<double>(value => {
            uploadProgressValue?.Invoke(value, size);
        });
        using var content = new MultipartFormDataContent
        {
            { new StringContent(toId.ToString()), "ToId" },
            { new StringContent(((sbyte)chatMessageSendType).ToString()), "SendType" },
            { new ProgressableStreamContent2(fileStream, progress), "File" },
        };
        var (succeeded, data, message) = await PostAsync<ChatMessageDto>("/api/chat/file", content, cancellationToken);
        return data;
    }

    public async Task<PagedList<UserDto>> SearchNickNameAsync(string searchContent, uint page = 1, uint size = 20, CancellationToken cancellationToken = default)
    {
        var (succeeded, data, message) = await GetJsonAsync<PagedList<UserDto>>($"/api/User/{page}/{size}?NickName={searchContent}", cancellationToken);
        return data;
    }

    public async Task FriendApplyAsync(FriendApplyRequest request, CancellationToken cancellationToken = default)
    {
        var (succeeded, message) = await PostJsonAsync("/api/FriendApply", request, cancellationToken);
    }

    public async Task<PagedList<ApplyPendingDto>> FriendApplyPendingAsync(FriendApplyPendingRequest request, CancellationToken cancellationToken = default)
    {
        var (succeeded, data, message) = await GetJsonAsync<PagedList<ApplyPendingDto>>(ApiClient.FormatParam($"/api/FriendApply/Pending", request), cancellationToken);
        return data;
    }

    public async Task<List<UserDto>> AllFriendAsync(CancellationToken cancellationToken = default)
    {
        var (succeeded, data, message) = await GetJsonAsync<List<UserDto>>("/api/Friend", cancellationToken);
        return data;
    }

    public async Task<QrAuthResponse> QrPreAuthAsync(string key, CancellationToken cancellationToken = default)
    {
        var (succeeded, data, message) = await PostJsonAsync<QrAuthResponse, object>($"/api/Auth/qr/preauth?key={key}", null, cancellationToken);
        return data;
    }

    public async Task<QrAuthResponse> QrAuthAsync(string key, CancellationToken cancellationToken = default)
    {
        var (succeeded, data, message) = await PostJsonAsync<QrAuthResponse, object>($"/api/Auth/qr/auth?key={key}", null, cancellationToken);
        return data;
    }

    public async Task<CreateQrAuthKeyResponse> CreateQrKeyAsync(CancellationToken cancellationToken = default)
    {
        var (succeeded, data, message) = await GetJsonAsync<CreateQrAuthKeyResponse>("/api/Auth/qr/key", cancellationToken);
        return data;
    }

    public async Task<CreateQrCodeResponse> CreateQrCodeAsync(CreateQrCodeRequest request, CancellationToken cancellationToken = default)
    {
        var (succeeded, data, message) = await PostJsonAsync<CreateQrCodeResponse, CreateQrCodeRequest>("/api/Auth/qr/create", request, cancellationToken);
        return data;
    }

    public async Task<CheckQrAuthKeyResponse> CheckQrKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        var (succeeded, data, message) = await GetJsonAsync<CheckQrAuthKeyResponse>($"/api/Auth/qr/check?key={key}", cancellationToken);
        return data;
    }

    private static string FormatParam(string url, object obj)
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
            sb.Append('=');
            sb.Append(HttpUtility.UrlEncode(v.ToString()));
            sb.Append('&');
        }
        sb.Remove(sb.Length - 1, 1);
        return sb.ToString();
    }

    public async Task<UserDto> FindUserAsync(int id, CancellationToken cancellationToken = default)
    {
        var (succeeded, data, message) = await GetJsonAsync<UserDto>($"/api/user/{id}", cancellationToken);
        return data;
    }
}

public class Download
{
    public DownloadState State { get; private set; }

    public double Uploaded { get; set; }

    public double Total { get; }

    public Download(double total)
    {
        Total = total;
    }

    public void ChangeState(DownloadState state)
    {
        State = state;
    }
}

public enum DownloadState
{
    UnReadly,
    PendingUpload,
    Uploading,
    PendingResponse
}

public class ProgressableStreamContent2 : HttpContent
{
    private const int defaultBufferSize = 4096;
    private Stream content;
    private int bufferSize;
    private bool contentConsumed;
    private IProgress<double> progress;

    public ProgressableStreamContent2(Stream content, IProgress<double> progress) : this(content, defaultBufferSize, progress)
    {
    }

    public ProgressableStreamContent2(Stream content, int bufferSize, IProgress<double> progress)
    {
        if (content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }
        if (bufferSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bufferSize));
        }

        this.content = content;
        this.bufferSize = bufferSize;
        this.progress = progress;
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
    {
        Contract.Assert(stream != null);

        PrepareContent();

        return Task.Run(() => {
            var buffer = new byte[bufferSize];
            var size = content.Length;
            var uploaded = 0;

            progress.Report(0);


            using (content)
            {
                while (true)
                {
                    var lenght = content.Read(buffer, 0, buffer.Length);
                    if (lenght <= 0) break;
                    progress.Report(uploaded += lenght);

                    stream.Write(buffer, 0, lenght);
                }
            }
        });
    }

    protected override bool TryComputeLength(out long length)
    {
        length = content.Length;
        return true;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            content.Dispose();
        }
        base.Dispose(disposing);
    }

    private void PrepareContent()
    {
        if (contentConsumed)
        {
            if (content.CanSeek)
            {
                content.Position = 0;
            }
            else
            {
                throw new InvalidOperationException("SR.net_http_content_stream_already_read");
            }
        }
        contentConsumed = true;
    }
}



/// <summary>
/// prgressable stream content.
/// used: https://stackoverflow.com/questions/35320238/how-to-display-upload-progress-using-c-sharp-httpclient-postasync
/// </summary>
public class ProgressableStreamContent : HttpContent
{
    private const int defaultBufferSize = 4096;
    private Stream content;
    private int bufferSize;
    private bool contentConsumed;
    private Download downloader;

    public ProgressableStreamContent(Stream content, Download downloader) : this(content, defaultBufferSize, downloader)
    {
    }

    public ProgressableStreamContent(Stream content, int bufferSize, Download downloader)
    {
        if (content is null)
        {
            throw new ArgumentNullException(nameof(content));
        }
        if (bufferSize < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(bufferSize));
        }

        this.content = content;
        this.bufferSize = bufferSize;
        this.downloader = downloader;
    }

    protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
    {
        Contract.Assert(stream != null);

        PrepareContent();

        return Task.Run(() => {
            var buffer = new Byte[bufferSize];
            var size = content.Length;
            var uploaded = 0;

            downloader.ChangeState(DownloadState.PendingUpload);

            using (content)
            {
                while (true)
                {
                    var lenght = content.Read(buffer, 0, buffer.Length);
                    if (lenght <= 0) break;

                    downloader.Uploaded = uploaded += lenght;

                    stream.Write(buffer, 0, lenght);

                    downloader.ChangeState(DownloadState.Uploading);
                }

                downloader.ChangeState(DownloadState.PendingResponse);
            }
        });
    }

    protected override bool TryComputeLength(out long length)
    {
        length = content.Length;
        return true;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            content.Dispose();
        }
        base.Dispose(disposing);
    }

    private void PrepareContent()
    {
        if (contentConsumed)
        {
            if (content.CanSeek)
            {
                content.Position = 0;
            }
            else
            {
                throw new InvalidOperationException("SR.net_http_content_stream_already_read");
            }
        }
        contentConsumed = true;
    }
}
