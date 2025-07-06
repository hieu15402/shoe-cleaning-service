using TP4SCS.Library.Models.Request.Chat;
using TP4SCS.Library.Models.Response.Chat;
using TP4SCS.Library.Models.Response.General;

namespace TP4SCS.Services.Interfaces
{
    public interface IChatService
    {
        Task<ApiResponse<ChatRoomResponse>> CreateChatRoomAsync(ChatRoomRequest roomRequest);

        Task<ApiResponse<IEnumerable<ChatRoomResponse>?>> GetChatRoomsAsync(int accId);

        Task<ApiResponse<MessageResponse>> SendMessageAsync(int id, MessageRequest messageRequest);

        Task<ApiResponse<MessageResponse>> SendOrderMessageAsync(int id, OrderMessageRequest orderMessageRequest);

        Task<ApiResponse<IEnumerable<MessageResponse>?>> GetMessagesAsync(int id, string roomId);

        Task<ApiResponse<string>> DeleteChatRoomAsync(string roomId);
    }
}
