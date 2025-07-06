using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.PlatformStatistic;
using TP4SCS.Library.Models.Response.PlatformStatistic;

namespace TP4SCS.Repository.Interfaces
{
    public interface IPlatformStatisticRepository : IGenericRepository<PlatformStatistic>
    {
        Task<PlatformStatisticResponse?> GetPlatformStatisticAsync(GetPlatformStatisticRequest getPlatformStatisticRequest);

        Task<PlatformStatisticResponse?> GetPlatformUserStatisticAsync();

        Task<PlatformStatistic?> GetPlatformUserStatisticByMonthAsync();

        Task<int[]?> GetPlatformOrderStatisticIdsByMonthAsync();

        Task<int[]?> GetPlatformOrderStatisticIdsByYearAsync();

        Task<int[]?> GetPlatformProfitStatisticIdsByMonthAsync();

        Task<int[]?> GetPlatformProfitStatisticIdsByYearAsync();

        Task<bool> CheckPlatformUserStatisticAsync();

        Task CreatePlatformStatisticAsync(List<PlatformStatistic> platformStatistics);

        Task UpdatePlatformStatisticAsync(PlatformStatistic platformStatistic);

        Task DeletePlatformStatisticAsync(List<int> ids);
    }
}
