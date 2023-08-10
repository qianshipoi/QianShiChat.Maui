namespace QianShiChatClient.Application.Models;

public partial class ChatMessageModel : ObservableObject
{
    public Guid LocalId { get; set; }
    public long Id { get; set; }
    public int FromId { get; set; }
    [ObservableProperty]
    private string? _fromAvatar;
    public int ToId { get; set; }
    [ObservableProperty]
    private string? _toAvatar;
    public ChatMessageSendType SendType { get; set; }
    public ChatMessageType MessageType { get; set; }
    public string Content { get; set; } = null!;
    public long CreateTime { get; set; }
    public bool IsSelfSend { get; set; }
    [ObservableProperty]
    private MessageStatus _status;
    [ObservableProperty]
    private double _uploadProgressValue;
}
