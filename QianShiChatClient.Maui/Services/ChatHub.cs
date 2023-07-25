namespace QianShiChatClient.Maui.Services;

public class ChatHub
{
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

    public ChatHub()
    {
        connection = new HubConnectionBuilder()
          .WithUrl($"{AppConsts.API_BASE_URL}/Hubs/Chat", options => {
              options.AccessTokenProvider = () => GetAccessToken();
              options.Headers.Add("User-Agent", "QianShiChatClient-Maui");
              options.Headers.Add("Client-Type", AppConsts.CLIENT_TYPE);
          })
          .WithAutomaticReconnect()
          .Build();

        connection.On<string, string>(nameof(ReceiveMessage), (u, n) => ReceiveMessage?.Invoke(u, n));
        connection.On<NotificationMessage>(nameof(Notification), (msg) => Notification?.Invoke(msg));
        connection.On<ChatMessageDto>(nameof(PrivateChat), (msg) => PrivateChat?.Invoke(msg));

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

    private Task<string> GetAccessToken() => Task.FromResult(Settings.AccessToken);

    public async Task Connect()
    {
        if (IsConnected || _isConnecting || string.IsNullOrWhiteSpace(await GetAccessToken())) return;

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