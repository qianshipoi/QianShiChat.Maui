namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class ChatMessageViewModel : ViewModelBase
{
    private readonly DataCenter _dataCenter;
    private readonly ILogger<ChatMessageViewModel> _logger;
    private readonly IDialogService _dialogService;

    [ObservableProperty]
    private Session _session;

    [ObservableProperty]
    private ChatMessage _toMessage;

    [ObservableProperty]
    private string _message;

    [ObservableProperty]
    private bool _scrollAnimated;

    public ChatMessageViewModel(
        DataCenter dataCenter,
        ILogger<ChatMessageViewModel> logger,
        IDialogService dialogService)
    {
        _dataCenter = dataCenter;
        _logger = logger;
        _dialogService = dialogService;
    }

    [RelayCommand]
    private async Task Send()
    {
        if (IsBusy || string.IsNullOrEmpty(Message)) return;

        IsBusy = true;
        try
        {
            var message = await _dataCenter.SendTextAsync(User, Session, Message);
            ScrollAnimated = true;
            ToMessage = message;
            Message = string.Empty;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "发送消息异常");
            await _dialogService.Toast("发送消息失败");
        }
        finally
        {
            IsBusy = false;
        }
    }
}