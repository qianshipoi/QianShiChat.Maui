namespace QianShiChatClient.Maui.ViewModels;

public sealed partial class MessageDetailViewModel : ViewModelBase, IQueryAttributable
{
    private readonly DataCenter _dataCenter;

    public string RoomId { get; private set; }

    [ObservableProperty]
    private RoomModelBase _room;

    [ObservableProperty]
    private MessageModel _toMessage;

    [ObservableProperty]
    private bool _scrollAnimated;

    [ObservableProperty]
    private string _message;

    public MessageDetailViewModel(
        DataCenter dataCenter)
    {
        _dataCenter = dataCenter;
    }

    [RelayCommand]
    private void Send()
    {
        if (IsBusy || string.IsNullOrEmpty(Message)) return;

        IsBusy = true;
        try
        {
            var message = _dataCenter.SendText(Room, Message);
            ScrollAnimated = true;
            ToMessage = message;
            Message = string.Empty;
        }
        finally
        {
            IsBusy = false;
        }
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        RoomId = (string)query[nameof(RoomId)];
        var room = _dataCenter.Rooms.FirstOrDefault(x => x.Id == RoomId);
        if (room != null)
        {
            Room = room;
        }
        else
        {
            Room = _dataCenter.Rooms.FirstOrDefault(x => x.Id == RoomId);
        }
    }
}