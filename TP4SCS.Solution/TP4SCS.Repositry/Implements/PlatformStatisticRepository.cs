using Microsoft.EntityFrameworkCore;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.PlatformStatistic;
using TP4SCS.Library.Models.Response.PlatformStatistic;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Repository.Interfaces;

namespace TP4SCS.Repository.Implements
{
    public class PlatformStatisticRepository : GenericRepository<PlatformStatistic>, IPlatformStatisticRepository
    {
        private readonly int _currentMonth = DateTime.Now.Month;
        private readonly int _currentYear = DateTime.Now.Year;

        public PlatformStatisticRepository(Tp4scsDevDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task CreatePlatformStatisticAsync(List<PlatformStatistic> platformStatistics)
        {
            await BulkInsertAsync(platformStatistics);
        }

        public async Task DeletePlatformStatisticAsync(List<int> ids)
        {
            await BulkDeleteAsync(ids);
        }

        public async Task<PlatformStatisticResponse?> GetPlatformUserStatisticAsync()
        {
            var statistics = await _dbContext.PlatformStatistics
                .AsNoTracking()
                .Where(s => s.Year == _currentYear &&
                s.Type.Equals(TypeConstants.USER))
                .Select(s => new
                {
                    s.Type,
                    Value = new PlatformStatisticValueResponse
                    {
                        Value = s.Value,
                        Date = null,
                        Month = s.Month,
                        Year = s.Year
                    }
                })
                .ToListAsync();

            if (!statistics.Any())
                return null;

            return new PlatformStatisticResponse
            {
                Type = statistics.First().Type,
                Value = statistics.Select(s => s.Value).ToList()
            };
        }


        public async Task<PlatformStatisticResponse?> GetPlatformStatisticAsync(GetPlatformStatisticRequest getPlatformStatisticRequest)
        {
            var statisticsQuery = _dbContext.PlatformStatistics.AsNoTracking().AsQueryable();

            if (!statisticsQuery.Any()) return null;

            if (getPlatformStatisticRequest.IsMonth)
            {
                switch (getPlatformStatisticRequest.Type)
                {
                    case StatisticOption.ORDER:
                        statisticsQuery = statisticsQuery.Where(s => s.IsMonth == true &&
                            s.Month == _currentMonth &&
                            s.Type.Equals(TypeConstants.ORDER));
                        break;
                    case StatisticOption.PROFIT:
                        statisticsQuery = statisticsQuery.Where(s => s.IsMonth == true &&
                            s.Month == _currentMonth &&
                            s.Type.Equals(TypeConstants.PROFIT));
                        break;
                }
            }
            else
            {
                switch (getPlatformStatisticRequest.Type)
                {
                    case StatisticOption.ORDER:
                        statisticsQuery = statisticsQuery.Where(s => s.IsYear == true &&
                            s.Year == _currentYear &&
                            s.Type.Equals(TypeConstants.ORDER));
                        break;
                    case StatisticOption.PROFIT:
                        statisticsQuery = statisticsQuery.Where(s => s.IsYear == true &&
                            s.Year == _currentYear &&
                            s.Type.Equals(TypeConstants.PROFIT));
                        break;
                }
            }

            var statistics = await statisticsQuery
                .Select(s => new
                {
                    s.Type,
                    Value = new PlatformStatisticValueResponse
                    {
                        Value = s.Value,
                        Date = s.Date,
                        Month = s.Month,
                        Year = s.Year
                    }
                })
                .ToListAsync();

            if (!statistics.Any())
                return null;

            return new PlatformStatisticResponse
            {
                Type = statistics.First().Type,
                Value = statistics.Select(s => s.Value).ToList()
            };
        }

        public async Task<bool> CheckPlatformUserStatisticAsync()
        {
            return await _dbContext.PlatformStatistics
                .AsNoTracking()
                .Where(s => s.Type.Equals(TypeConstants.USER) &&
                    s.Year == _currentYear)
                .AnyAsync();
        }

        public async Task<PlatformStatistic?> GetPlatformUserStatisticByMonthAsync()
        {
            return await _dbContext.PlatformStatistics
                .Where(s => s.IsMonth == true &&
                s.Month == _currentMonth &&
                s.Type.Equals(TypeConstants.USER))
                .FirstOrDefaultAsync();
        }

        public async Task UpdatePlatformStatisticAsync(PlatformStatistic platformStatistic)
        {
            await UpdateAsync(platformStatistic);
        }

        public async Task<int[]?> GetPlatformOrderStatisticIdsByMonthAsync()
        {
            return await _dbContext.PlatformStatistics
                .AsNoTracking()
                .Where(s => s.IsMonth == true &&
                    s.Month == _currentMonth &&
                    s.Year == _currentYear &&
                    s.Type.Equals(TypeConstants.ORDER))
                .Select(s => s.Id)
                .ToArrayAsync();
        }

        public async Task<int[]?> GetPlatformOrderStatisticIdsByYearAsync()
        {
            return await _dbContext.PlatformStatistics
                .AsNoTracking()
                .Where(s => s.IsYear == true &&
                    s.Year == _currentYear &&
                    s.Type.Equals(TypeConstants.ORDER))
                .Select(s => s.Id)
                .ToArrayAsync();
        }

        public async Task<int[]?> GetPlatformProfitStatisticIdsByMonthAsync()
        {
            return await _dbContext.PlatformStatistics
                .AsNoTracking()
                .Where(s => s.IsMonth == true &&
                    s.Month == _currentMonth &&
                    s.Year == _currentYear &&
                    s.Type.Equals(TypeConstants.PROFIT))
                .Select(s => s.Id)
                .ToArrayAsync();
        }

        public async Task<int[]?> GetPlatformProfitStatisticIdsByYearAsync()
        {
            return await _dbContext.PlatformStatistics
                .AsNoTracking()
                .Where(s => s.IsYear == true &&
                    s.Year == _currentYear &&
                    s.Type.Equals(TypeConstants.PROFIT))
                .Select(s => s.Id)
                .ToArrayAsync();
        }
    }
}
