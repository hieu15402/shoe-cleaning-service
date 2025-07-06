using Microsoft.EntityFrameworkCore;
using TP4SCS.Library.Models.Data;
using TP4SCS.Repository.Interfaces;

namespace TP4SCS.Repository.Implements
{
    public class PackSubscriptionRepository : GenericRepository<PackSubscription>, IPackSubscriptionRepository
    {
        public PackSubscriptionRepository(Tp4scsDevDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task<bool> CheckRegisteredPackOfBusinessAsync(int id, string feature)
        {
            return await _dbContext.PackSubscriptions
                .AsNoTracking()
                .Include(x => x.Pack)
                .AnyAsync(s => s.BusinessId == id && s.Pack.Feature!.Equals(feature));
        }

        public async Task CreatePackSubscriptionAsync(PackSubscription packSubscription)
        {
            await InsertAsync(packSubscription);
        }

    }
}
