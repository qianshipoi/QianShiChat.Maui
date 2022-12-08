namespace QianShiChatClient.Maui.ViewModels;

[QueryProperty(nameof(Session), nameof(Session))]
public sealed partial class MessageDetailViewModel : ViewModelBase
{
    readonly IApiClient _apiClient;
    readonly ChatDatabase _database;

    [ObservableProperty]
    Session _session;

    public MessageDetailViewModel(
        IStringLocalizer<MyStrings> stringLocalizer,
        INavigationService navigationService,
        IApiClient apiClient,
        ChatDatabase database)
        : base(navigationService, stringLocalizer)
    {
        _apiClient = apiClient;
        _database = database;
    }

    [ObservableProperty]
    string _message;

    [RelayCommand]
    async Task Send()
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
            Session.AddMessage(message);

            await _database.SaveChatMessageAsnyc(message);
            Message = string.Empty;
        }
        finally
        {
            IsBusy = false;
        }
    }
}
