using TP4SCS.Library.Models.Request.Ticket;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.Ticket;

namespace TP4SCS.Services.Interfaces
{
    public interface ITicketService
    {
        Task<ApiResponse<IEnumerable<TicketsResponse>?>> GetTicketsAsync(GetTicketRequest getTicketRequest);

        Task<ApiResponse<IEnumerable<TicketsResponse>?>> GetTicketsByBranchIdAsync(int id, GetBusinessTicketRequest getBusinessTicketRequest);

        Task<ApiResponse<IEnumerable<TicketsResponse>?>> GetTicketsByBusinessAsync(int id, GetBusinessTicketRequest getBusinessTicketRequest);

        Task<ApiResponse<TicketResponse?>> GetTicketByIdAsync(int id);

        Task<ApiResponse<TicketResponse>> CreateTicketAsync(int id, CreateTicketRequest createTicketRequest);

        Task<ApiResponse<TicketResponse>> CreateOrderTicketAsync(int id, CreateOrderTicketRequest createOrderTicketRequest);

        Task<ApiResponse<TicketResponse>> CreateChildTicketAsync(int userid, int id, CreateChildTicketRequest createChildTicketRequest);

        Task<ApiResponse<TicketResponse>> UpdateTicketStatusAsync(int moderatorId, int id, UpdateTicketStatusRequest updateTicketStatusRequest);

        Task<ApiResponse<TicketResponse>> CancelTicketAsync(int id);

        Task<ApiResponse<TicketResponse>> NotifyForCustomerAsync(NotifyTicketForCustomerRequest notifyTicketForCustomerRequest);

        Task<ApiResponse<TicketResponse>> NotifyForOwnerAsync(NotifyTicketForOwnerRequest notifyTicketForOwnerRequest);
    }
}
