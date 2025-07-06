using TP4SCS.Library.Models.Data;

namespace TP4SCS.Repository.Interfaces
{
    public interface IBranchRepository : IGenericRepository<BusinessBranch>
    {
        Task<IEnumerable<BusinessBranch>?> GetBranchesByBusinessIdAsync(int id);

        Task<int[]?> GetBranchesIdByOwnerIdAsync(int id);

        Task<BusinessBranch?> GetBranchByIdAsync(int id);

        Task<int?> GetBranchIdByEmployeeIdAsync(int id);

        Task<int> GetBranchMaxIdAsync();

        Task<int> CountBranchDataByBusinessIdAsync(int id);

        Task CreateBranchAsync(BusinessBranch businessBranch);

        Task UpdateBranchAsync(BusinessBranch businessBranch);
    }
}
