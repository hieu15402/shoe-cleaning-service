using Microsoft.EntityFrameworkCore;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Repository.Interfaces;

namespace TP4SCS.Repository.Implements
{
    public class PlatformPackRepository : GenericRepository<PlatformPack>, IPlatformPackRepository
    {
        public PlatformPackRepository(Tp4scsDevDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task<int> CountPackAsync()
        {
            return await _dbContext.PlatformPacks.AsNoTracking().CountAsync();
        }

        public async Task CreatePackAsync(PlatformPack subscriptionPack)
        {
            await InsertAsync(subscriptionPack);
        }

        public async Task<PlatformPack?> GetPackByNameAsync(string name)
        {
            return await _dbContext.PlatformPacks
                .AsNoTracking()
                .FirstOrDefaultAsync(p => EF.Functions.Collate(p.Name, "SQL_Latin1_General_CP1_CI_AS")
                .Equals(name));
        }

        public async Task<decimal> GetPackPriceByPeriodAsync(int period)
        {
            return await _dbContext.PlatformPacks.AsNoTracking().Where(p => p.Period == period).Select(p => p.Price).SingleOrDefaultAsync();
        }

        public async Task<int> GetPackMaxIdAsync()
        {
            return await _dbContext.PlatformPacks.AsNoTracking().Where(p => p.Period != 0).MaxAsync(p => p.Id);
        }

        public async Task<IEnumerable<PlatformPack>?> GetRegisterPacksAsync()
        {
            return await _dbContext.PlatformPacks
                .AsNoTracking()
                .Where(p => p.Type.Equals(TypeConstants.REGISTER))
                .OrderBy(p => p.Period)
                .ToListAsync();
        }

        public async Task<List<int>> GetPeriodArrayAsync()
        {
            return await _dbContext.PlatformPacks.AsNoTracking().Where(p => p.Period != 0).Select(p => p.Period).ToListAsync();
        }

        public async Task<bool> IsPackNameExistedAsync(string name)
        {
            return await _dbContext.PlatformPacks
                .AsNoTracking()
                .AnyAsync(p => EF.Functions.Collate(p.Name, "SQL_Latin1_General_CP1_CI_AS")
                .Equals(name)
                && p.Period != 0);
        }

        public async Task UpdatePackAsync(PlatformPack subscriptionPack)
        {
            await UpdateAsync(subscriptionPack);
        }

        public async Task<PlatformPack?> GetPackByIdAsync(int id)
        {
            return await _dbContext.PlatformPacks.SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<PlatformPack?> GetPackByIdNoTrackingAsync(int id)
        {
            return await _dbContext.PlatformPacks.AsNoTracking().SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task DeletePackAsync(int id)
        {
            await DeleteAsync(id);
        }

        public async Task<PlatformPack?> GetPackByPeriodAsync(int period)
        {
            return await _dbContext.PlatformPacks.FirstOrDefaultAsync(p => p.Period == period);
        }

        public async Task<IEnumerable<PlatformPack>?> GetFeaturePacksAsync()
        {
            return await _dbContext.PlatformPacks
                .AsNoTracking()
                .Where(p => p.Type.Equals(TypeConstants.FEATURE))
                .OrderBy(p => p.Period)
                .ToListAsync();
        }
    }
}
