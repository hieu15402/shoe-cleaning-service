using TP4SCS.Library.Models.Data;

namespace TP4SCS.Repository.Interfaces
{
    public interface ITicketCategoryRepository : IGenericRepository<TicketCategory>
    {
        Task<IEnumerable<TicketCategory>?> GetCategoriesAsync();

        Task<TicketCategory?> GetCategoryByIdAsync(int id);

        Task<int> GetOrderTicketCategoryIdAsync();

        Task<bool> IsNameExistedAsync(string name);

        Task CreateCategoryAsync(TicketCategory category);

        Task UpdateCategoryAsync(TicketCategory category);
    }
}
