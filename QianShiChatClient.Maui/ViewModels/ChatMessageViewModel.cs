namespace QianShiChatClient.Maui.ViewModels
{
    public sealed partial class ChatMessageViewModel : ViewModelBase
    {
        private readonly IApiClient _apiClient;
        private readonly IUserService _userService;
        private readonly ChatDatabase _database;
        private readonly DataCenter _dataCenter;

        [ObservableProperty]
        private Session _session;

        [ObservableProperty]
        private ChatMessage _toMessage;

        [ObservableProperty]
        private string _message;

        private bool _isNewSession;

        [ObservableProperty]
        private bool _scrollAnimated;

        public ChatMessageViewModel(
            INavigationService navigationService, 
            IStringLocalizer<MyStrings> stringLocalizer, 
            IApiClient apiClient, 
            IUserService userService, 
            ChatDatabase database, 
            DataCenter dataCenter) 
            : base(navigationService, stringLocalizer)
        {
            _apiClient = apiClient;
            _userService = userService;
            _database = database;
            _dataCenter = dataCenter;
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
                if (_isNewSession)
                {
                    _dataCenter.Sessions.Add(Session);
                }
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
