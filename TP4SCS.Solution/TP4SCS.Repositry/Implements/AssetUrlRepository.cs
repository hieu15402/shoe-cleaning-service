using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Repository.Interfaces;

namespace TP4SCS.Repository.Implements
{
    public class AssetUrlRepository : GenericRepository<AssetUrl>, IAssetUrlRepository

    {
        public AssetUrlRepository(Tp4scsDevDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task AddAssetUrlsAsync(List<AssetUrl> assetUrls)
        {
            await _dbContext.AssetUrls.AddRangeAsync(assetUrls);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAssetUrlAsync(int id)
        {
            await DeleteAsync(id);
        }

        public Task<IEnumerable<AssetUrl>?> GetAssetUrlsAsync(
            int? pageIndex = null,
            int? pageSize = null,
            OrderByEnum orderBy = OrderByEnum.IdAsc)
        {
            Func<IQueryable<AssetUrl>, IOrderedQueryable<AssetUrl>> orderByExpression = q => orderBy switch
            {
                OrderByEnum.IdDesc => q.OrderByDescending(c => c.Id),
                _ => q.OrderBy(c => c.Id)
            };

            // Check for pagination
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                // Fetch paginated services
                return GetAsync(
                    orderBy: orderByExpression,
                    pageIndex: pageIndex.Value,
                    pageSize: pageSize.Value
                );
            }

            // Fetch all services without pagination
            return GetAsync(
                orderBy: orderByExpression
            );
        }

        public async Task<AssetUrl?> GetAssetUrlByIdAsync(int id)
        {
            return await GetByIDAsync(id);
        }
        public async Task UpdateAssetUrlsAsync(List<AssetUrl> assetUrls)
        {
            foreach (var assetUrl in assetUrls)
            {
                await UpdateAsync(assetUrl);
            }
        }


        public async Task UpdateAssetUrlAsync(AssetUrl assetUrl)
        {
            await UpdateAsync(assetUrl);
        }
    }
}
