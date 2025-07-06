using Microsoft.EntityFrameworkCore;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Repository.Interfaces;

namespace TP4SCS.Repository.Implements
{
    public class BranchRepository : GenericRepository<BusinessBranch>, IBranchRepository
    {
        public BranchRepository(Tp4scsDevDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task<int> CountBranchDataByBusinessIdAsync(int id)
        {
            return await _dbContext.BusinessBranches.AsNoTracking().CountAsync(b => b.BusinessId == id);
        }

        public async Task CreateBranchAsync(BusinessBranch businessBranch)
        {
            await InsertAsync(businessBranch);
            var branchId = businessBranch.Id;
            var servicesByBusinessId = _dbContext.Services
                .Include(s => s.BranchServices)
                    .ThenInclude(bs => bs.Branch)
                .Where(s => s.BranchServices.Any(bs => bs.Branch.BusinessId == businessBranch.BusinessId));
            foreach (var service in servicesByBusinessId)
            {
                var branchService = new BranchService
                {
                    BranchId = branchId,
                    ServiceId = service.Id,
                    Status = StatusConstants.UNAVAILABLE
                };

                _dbContext.BranchServices.Add(branchService);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task<BusinessBranch?> GetBranchByIdAsync(int id)
        {
            return await _dbContext.BusinessBranches.SingleOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<BusinessBranch>?> GetBranchesByBusinessIdAsync(int id)
        {
            return await _dbContext.BusinessBranches.Where(b => b.BusinessId == id).ToListAsync();
        }

        public async Task<int[]?> GetBranchesIdByOwnerIdAsync(int id)
        {
            return await _dbContext.BusinessProfiles
                .AsNoTracking()
                .Where(p => p.OwnerId == id)
                .SelectMany(p => p.BusinessBranches)
                .Select(b => b.Id)
                .ToArrayAsync();
        }

        public async Task<int?> GetBranchIdByEmployeeIdAsync(int id)
        {
            return await _dbContext.BusinessBranches
                .AsNoTracking()
                .Where(b => EF.Functions.Like(b.EmployeeIds, $"%{id.ToString()}%"))
                .Select(b => b.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetBranchMaxIdAsync()
        {
            return await _dbContext.BusinessBranches.AsNoTracking().MaxAsync(b => b.Id);
        }

        public async Task UpdateBranchAsync(BusinessBranch businessBranch)
        {
            await UpdateAsync(businessBranch);
        }
    }
}
