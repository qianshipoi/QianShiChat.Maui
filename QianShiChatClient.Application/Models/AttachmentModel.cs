namespace QianShiChatClient.Application.Models;

public partial class AttachmentModel : ObservableObject
{
    [ObservableProperty]
    private long _id;
    [ObservableProperty]
    private string _name = string.Empty;
    [ObservableProperty]
    private string _rawPath = string.Empty;
    [ObservableProperty]
    private string? _previewPath = string.Empty;
    [ObservableProperty]
    private string _hash = string.Empty;
    [ObservableProperty]
    private string _contentType = string.Empty;
    [ObservableProperty]
    private long _size;
    [ObservableProperty]
    private double _progress;
}
