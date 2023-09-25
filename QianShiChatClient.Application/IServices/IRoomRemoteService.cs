namespace QianShiChatClient.Application.IServices;

public interface IRoomRemoteService
{
    Task<GroupDto?> GetGroupByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PagedList<ChatMessageDto>> GetHistoryAsync(string roomId, int page, int size, CancellationToken cancellationToken = default);
    Task<RoomDto?> GetRoomAsync(int toId, ChatMessageSendType type);
    IAsyncEnumerable<RoomDto> GetRoomsAsync(CancellationToken cancellationToken = default);
}
