using TP4SCS.Library.Models.Data;

namespace TP4SCS.Repository.Interfaces
{
    public interface IPlatformPackRepository : IGenericRepository<PlatformPack>
    {
        Task<IEnumerable<PlatformPack>?> GetRegisterPacksAsync();

        Task<IEnumerable<PlatformPack>?> GetFeaturePacksAsync();

        Task<PlatformPack?> GetPackByIdAsync(int id);

        Task<PlatformPack?> GetPackByPeriodAsync(int period);

        Task<PlatformPack?> GetPackByIdNoTrackingAsync(int id);

        Task<PlatformPack?> GetPackByNameAsync(string name);

        Task<decimal> GetPackPriceByPeriodAsync(int period);

        Task<int> GetPackMaxIdAsync();

        Task<int> CountPackAsync();

        Task<List<int>> GetPeriodArrayAsync();

        Task<bool> IsPackNameExistedAsync(string name);

        Task CreatePackAsync(PlatformPack PlatformPack);

        Task UpdatePackAsync(PlatformPack PlatformPack);

        Task DeletePackAsync(int id);
    }
}
