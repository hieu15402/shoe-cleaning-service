using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Ticket;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.Ticket;

namespace TP4SCS.Repository.Interfaces
{
    public interface ITicketRepository : IGenericRepository<SupportTicket>
    {
        Task<(IEnumerable<TicketsResponse>?, Pagination)> GetTicketsAsync(GetTicketRequest getTicketRequest);

        Task<(IEnumerable<TicketsResponse>?, Pagination)> GetTicketsByBranchIdAsync(int id, GetBusinessTicketRequest getBusinessTicketRequest);

        Task<(IEnumerable<TicketsResponse>?, Pagination)> GetTicketsByBusinessIdAsync(int id, GetBusinessTicketRequest getBusinessTicketRequest);

        Task<SupportTicket?> GetModeratorChildTicketByParentId(int id);

        Task<TicketResponse?> GetTicketByIdAsync(int id);

        Task<SupportTicket?> GetUpdateTicketByIdAsync(int id);

        Task CreateTicketAsync(SupportTicket supportTicket);

        Task UpdateTicketAsync(SupportTicket supportTicket);

        Task DeleteTicketAsync(int id);
    }
}
