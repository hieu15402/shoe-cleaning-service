using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;

namespace TP4SCS.Services.Interfaces
{
    public interface IServiceCategoryService
    {
        Task<ServiceCategory?> GetServiceCategoryByIdAsync(int id);
        Task<IEnumerable<ServiceCategory>?> GetServiceCategoriesAsync(string? keyword = null, string? status = null,
            int pageIndex = 1, int pageSize = 5, OrderByEnum orderBy = OrderByEnum.IdAsc);
        Task AddServiceCategoryAsync(ServiceCategory category);
        Task<int> GetTotalServiceCategoriesCountAsync(string? keyword = null, string? status = null);
        Task UpdateServiceCategoryAsync(ServiceCategory category, int existingCategoryId);
        Task DeleteServiceCategoryAsync(int id);
    }
}
