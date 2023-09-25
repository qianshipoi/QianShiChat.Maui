namespace QianShiChatClient.Application.Models;

public partial class MessageModel : ObservableObject
{
    [ObservableProperty]
    private long _id;
    [ObservableProperty]
    private int _fromId;
    [ObservableProperty]
    private int toId;
    [ObservableProperty]
    private ChatMessageSendType _sendType;
    [ObservableProperty]
    private ChatMessageType _messageType;
    [ObservableProperty]
    private string _content = string.Empty;
    [ObservableProperty]
    private long _createTime;
    [ObservableProperty]
    private MessageStatus _status;
    public UserInfoModel FromUser { get; init; }
    public bool IsSelfSend { get; init; }
    public ObservableCollection<AttachmentModel> Attachments;

    public MessageModel(UserInfoModel fromUser, bool isSelf, IEnumerable<AttachmentModel>? attachments = null)
    {
        FromUser = fromUser;
        Attachments = new ObservableCollection<AttachmentModel>(attachments ?? new List<AttachmentModel>());
        IsSelfSend = isSelf;
    }
}
