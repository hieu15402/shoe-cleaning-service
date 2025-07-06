using TP4SCS.Library.Models.Request.Category;
using TP4SCS.Library.Models.Response.Category;
using TP4SCS.Library.Models.Response.General;

namespace TP4SCS.Services.Interfaces
{
    public interface ITicketCategoryService
    {
        Task<ApiResponse<IEnumerable<TicketCategoryResponse>?>> GetCategoriesAsync();

        Task<ApiResponse<TicketCategoryResponse?>> GetCategoryByIdAsync(int id);

        Task<ApiResponse<TicketCategoryResponse>> CreateCategoryAsync(TicketCategoryRequest ticketCategoryRequest);

        Task<ApiResponse<TicketCategoryResponse>> UpdateCategoryAsync(int id, UpdateTicketCategoryRequest updateTicketCategoryRequest);
    }
}
