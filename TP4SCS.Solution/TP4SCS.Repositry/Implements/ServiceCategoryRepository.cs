using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Repository.Interfaces;

namespace TP4SCS.Repository.Implements
{
    public class ServiceCategoryRepository : GenericRepository<ServiceCategory>, IServiceCategoryRepository
    {
        public ServiceCategoryRepository(Tp4scsDevDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task AddCategoryAsync(ServiceCategory category)
        {
            await InsertAsync(category);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            await DeleteAsync(id);
        }
        public async Task<int> GetTotalCategoriesCountAsync(string? keyword = null, string? status = null)
        {
            Expression<Func<ServiceCategory, bool>> filter = s =>
                (string.IsNullOrEmpty(keyword) || s.Name.ToLower().Contains(keyword.ToLower().Trim())) &&
                (string.IsNullOrEmpty(status) || s.Status.ToLower().Trim() == status.ToLower().Trim());

            return await _dbContext.ServiceCategories.AsNoTracking().CountAsync(filter);
        }
        public Task<IEnumerable<ServiceCategory>?> GetCategoriesAsync(
            string? keyword = null,
            string? status = null,
            int pageIndex = 1,
            int pageSize = 5,
            OrderByEnum orderBy = OrderByEnum.IdAsc)
        {
            // Biểu thức lọc với cả keyword và status
            Expression<Func<ServiceCategory, bool>> filter = s =>
                (string.IsNullOrEmpty(keyword) || s.Name.Contains(keyword)) &&
                (string.IsNullOrEmpty(status) || s.Status.ToLower() == status.ToLower());

            Func<IQueryable<ServiceCategory>, IOrderedQueryable<ServiceCategory>> orderByExpression = q => orderBy switch
            {
                OrderByEnum.IdDesc => q.OrderByDescending(c => c.Id),
                _ => q.OrderBy(c => c.Id)
            };
            return GetAsync(
                filter: filter,
                orderBy: orderByExpression,
                pageIndex: pageIndex,
                pageSize: pageSize
            );
        }

        public async Task<ServiceCategory?> GetCategoryByIdAsync(int id)
        {
            return await GetByIDAsync(id);
        }

        public async Task UpdateCategoryAsync(ServiceCategory category)
        {
            await UpdateAsync(category);
        }
    }
}
