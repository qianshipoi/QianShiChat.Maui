using Microsoft.AspNetCore.SignalR.Client;

namespace QianShiChatClient.Maui.Services;

public class ChatHub
{
    private readonly IApiClient _apiClient;

    private HubConnection connection;
    private bool _isConnected;
    private bool _isConnecting;

    public event Action<string, string> ReceiveMessage;

    public event Action<NotificationMessage> Notification;

    public event Action<ChatMessageDto> PrivateChat;

    public event Action<bool> IsConnectedChanged;

    public bool IsConnected
    {
        get => _isConnected;
        private set
        {
            if (_isConnected != value)
            {
                _isConnected = value;
                IsConnectedChanged?.Invoke(value);
            }
        }
    }

    public ChatHub(IApiClient apiClient)
    {
        _apiClient = apiClient;

        connection = new HubConnectionBuilder()
          .WithUrl($"{ApiClient.BaseAddress}/Hubs/Chat", options => {
              options.AccessTokenProvider = () => GetAccessToken();
              options.Headers.Add("Client-Type", _apiClient.ClientType);
          })
          .WithAutomaticReconnect()
          .Build();

        connection.On<string, string>("ReceiveMessage", (u, n) => ReceiveMessage?.Invoke(u, n));
        connection.On<NotificationMessage>("Notification", (msg) => Notification?.Invoke(msg));
        connection.On<ChatMessageDto>("PrivateChat", (msg) => PrivateChat?.Invoke(msg));

        connection.Closed += async (error) => {
            IsConnected = false;
            _isConnecting = false;
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await Connect();
        };

        connection.Reconnecting += (msg) => {
            IsConnected = false;
            _isConnecting = true;
            return Task.CompletedTask;
        };

        connection.Reconnected += (msg) => {
            IsConnected = true;
            _isConnecting = false;
            return Task.CompletedTask;
        };
    }

    private Task<string> GetAccessToken() => Task.FromResult(_apiClient.AccessToken);

    public async Task Connect()
    {
        if (IsConnected || _isConnecting || string.IsNullOrWhiteSpace(_apiClient.AccessToken)) return;

        _isConnecting = true;
        try
        {
            await connection.StartAsync();
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }
        finally
        {
            _isConnecting = false;
        }
        IsConnected = true;
    }

    public async Task Deconnect()
    {
        if (!IsConnected) return;
        await connection.StopAsync();
        IsConnected = false;
    }
}
