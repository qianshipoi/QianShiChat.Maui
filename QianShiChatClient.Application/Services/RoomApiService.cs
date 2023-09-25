using QianShiChatClient.Application.IServices;

namespace QianShiChatClient.Application.Services;

public class RoomApiService 
{
    private readonly IApiClient _apiClient;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _serializerOptions;

    public RoomApiService(IHttpClientFactory httpClientFactory, IApiClient apiClient)
    {
        _httpClientFactory = httpClientFactory;
        _apiClient = apiClient;
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }
}

