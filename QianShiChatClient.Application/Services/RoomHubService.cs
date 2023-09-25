namespace QianShiChatClient.Application.Services;

public class RoomHubService : IRoomRemoteService
{
    private readonly ILogger<RoomHubService> _logger;
    private readonly ChatHub _chatHub;
    private readonly IApiClient _apiClient;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly ApiOptions _options;

    public RoomHubService(
        ChatHub chatHub,
        ILogger<RoomHubService> logger,
        IHttpClientFactory httpClientFactory,
        IApiClient apiClient,
        IOptions<ApiOptions> options)
    {
        _chatHub = chatHub;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _apiClient = apiClient;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        _options = options.Value;
    }

    public IAsyncEnumerable<RoomDto> GetRoomsAsync(CancellationToken cancellationToken = default)
    {
        return _chatHub.HubConnection.StreamAsync<RoomDto>("GetRoomsAsync", cancellationToken);
    }

    public async Task<RoomDto?> GetRoomAsync(int toId, ChatMessageSendType type)
    {
        try
        {
            return await _chatHub.HubConnection.InvokeAsync<RoomDto>("GetRoomAsync", toId, type);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "get room error");
            return null;
        }
    }

    public async Task<PagedList<ChatMessageDto>> GetHistoryAsync(string roomId, int page, int size, CancellationToken cancellationToken = default)
    {
        using var client = _httpClientFactory.CreateClient(AppConsts.API_CLIENT_NAME);
        var result = await client.GetFromJsonAsync<GlobalResult<PagedList<ChatMessageDto>>>($"/api/chat/{roomId}/history?page={page}&size={size}", _serializerOptions, cancellationToken);
        if (result is null)
        {
            throw new Exception("get history error");
        }
        return result.Data;
    }

    public async Task<GroupDto?> GetGroupByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var client = _httpClientFactory.CreateClient(_options.ClientName);
        var response = await client.GetFromJsonAsync<GlobalResult<GroupDto>>($"/api/group/{id}", _serializerOptions, cancellationToken);
        if (response is null) return null;

        return response.Data;
    }
}

