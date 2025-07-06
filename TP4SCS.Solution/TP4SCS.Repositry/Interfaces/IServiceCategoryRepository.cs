using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;

namespace TP4SCS.Repository.Interfaces
{
    public interface IServiceCategoryRepository
    {
        Task<IEnumerable<ServiceCategory>?> GetCategoriesAsync(
            string? keyword = null,
            string? status = null,
            int pageIndex = 1,
            int pageSize = 5,
            OrderByEnum orderBy = OrderByEnum.IdAsc);
        Task<ServiceCategory?> GetCategoryByIdAsync(int id);
        Task<int> GetTotalCategoriesCountAsync(string? keyword = null, string? status = null);
        Task AddCategoryAsync(ServiceCategory category);
        Task UpdateCategoryAsync(ServiceCategory category);
        Task DeleteCategoryAsync(int id);
    }
}
