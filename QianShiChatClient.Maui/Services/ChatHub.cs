using Microsoft.AspNetCore.SignalR.Client;

namespace QianShiChatClient.Maui.Services;

public class ChatHub
{
    readonly IApiClient _apiClient;

    HubConnection connection;

    bool _isConnected;

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
          .WithUrl($"{ApiClient.BaseAddress}/Hubs/Chat", options =>
          {
              options.AccessTokenProvider = () => GetAccessToken();
          })
          .WithAutomaticReconnect()
          .Build();

        connection.Closed += async (error) =>
        {
            IsConnected = false;
            await Task.Delay(new Random().Next(0, 5) * 1000);
            await connection.StartAsync();

        };

        connection.Reconnecting += (msg) =>
        {
            IsConnected = false;
            return Task.CompletedTask;
        };

        connection.Reconnected += (msg) =>
        {
            IsConnected = true;
            return Task.CompletedTask;
        };
    }

    Task<string> GetAccessToken() => Task.FromResult(_apiClient.AccessToken);

    public async Task Connect()
    {
        if (IsConnected) return;

        connection.On<string, string>("ReceiveMessage", (u, n) => ReceiveMessage?.Invoke(u, n));
        connection.On<NotificationMessage>("Notification", (msg) => Notification?.Invoke(msg));
        connection.On<ChatMessageDto>("PrivateChat", (msg) => PrivateChat?.Invoke(msg));

        try
        {
            await connection.StartAsync();
        }
        catch (Exception ex)
        {
            await Toast.Make(ex.Message).Show();
        }

        IsConnected = true;
    }
}
