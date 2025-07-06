using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Repository.Interfaces;

namespace TP4SCS.Repository.Implements
{
    public class PromotionRepository : GenericRepository<Promotion>, IPromotionRepository
    {
        public PromotionRepository(Tp4scsDevDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task AddPromotionAsync(Promotion promotion)
        {
            await InsertAsync(promotion);
        }

        public async Task DeletePromotionAsync(int id)
        {
            await DeleteAsync(id);
        }

        public async Task<Promotion?> GetPromotionByIdAsync(int id)
        {
            return await GetByIDAsync(id);
        }
        public async Task<int> GetTotalPromotionsCountAsync(string? keyword = null, string? status = null)
        {
            Expression<Func<Promotion, bool>> filter = s =>
                (string.IsNullOrEmpty(keyword) || s.Service.Name.ToLower().Trim().Contains(keyword.ToLower().Trim())) &&
                (string.IsNullOrEmpty(status) || s.Status.ToLower().Trim() == status.ToLower().Trim());

            return await _dbContext.Promotions.AsNoTracking().CountAsync(filter);
        }
        public Task<IEnumerable<Promotion>?> GetPromotionsAsync(
            string? keyword = null,
            string? status = null,
            int pageIndex = 1,
            int pageSize = 5,
            OrderByEnum orderBy = OrderByEnum.IdAsc)
        {
            // Filter by keyword and status
            Expression<Func<Promotion, bool>> filter = p =>
                (string.IsNullOrEmpty(keyword) || p.Service.Name.Contains(keyword)) &&
                (string.IsNullOrEmpty(status) || p.Status.ToLower() == status.ToLower());

            // Sort based on OrderByEnum
            Func<IQueryable<Promotion>, IOrderedQueryable<Promotion>> orderByExpression = q => orderBy switch
            {
                OrderByEnum.IdDesc => q.OrderByDescending(p => p.Id),
                _ => q.OrderBy(p => p.Id)
            };

            // Fetch paginated promotions
            return GetAsync(
                filter: filter,
                orderBy: orderByExpression,
                pageIndex: pageIndex,
                pageSize: pageSize
            );
        }

        public async Task UpdatePromotionAsync(Promotion promotion)
        {
            await UpdateAsync(promotion);
        }
    }

}
