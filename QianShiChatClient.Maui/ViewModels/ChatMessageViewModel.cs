namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class ChatMessageViewModel : ViewModelBase
{
    private readonly IApiClient _apiClient;
    private readonly IUserService _userService;
    private readonly ChatDatabase _database;

    [ObservableProperty]
    private Session _session;

    [ObservableProperty]
    private ChatMessage _toMessage;

    [ObservableProperty]
    private string _message;

    [ObservableProperty]
    private bool _scrollAnimated;

    public ChatMessageViewModel(
        IApiClient apiClient,
        IUserService userService,
        ChatDatabase database)
    {
        _apiClient = apiClient;
        _userService = userService;
        _database = database;
    }

    [RelayCommand]
    private async Task Send()
    {
        if (IsBusy || string.IsNullOrEmpty(Message)) return;

        IsBusy = true;
        try
        {
            var chatDto = await _apiClient
                .SendTextAsync(new PrivateChatMessageRequest(
                    Session.User.Id,
                    Message,
                    ChatMessageSendType.Personal));
            chatDto.IsSelfSend = true;
            var message = chatDto.ToChatMessage();
            message.FromAvatar = (await _userService.GetUserInfoByIdAsync(message.FromId)).Avatar;
            message.ToAvatar = (await _userService.GetUserInfoByIdAsync(message.ToId)).Avatar;
            Session.AddMessage(message);
            await _database.SaveChatMessageAsnyc(message);
            ScrollAnimated = true;
            ToMessage = message;
            Message = string.Empty;
        }
        finally
        {
            IsBusy = false;
        }
    }
}