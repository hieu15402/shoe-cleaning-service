using Microsoft.EntityFrameworkCore;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Response.BusinessStatistic;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Repository.Interfaces;

namespace TP4SCS.Repository.Implements
{
    public class BusinessStatisticRepository : GenericRepository<BusinessStatistic>, IBusinessStatisticRepository
    {
        public BusinessStatisticRepository(Tp4scsDevDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task CreateBusinessStatisticAsync(List<BusinessStatistic> businessStatistic)
        {
            await BulkInsertAsync(businessStatistic);
        }

        public async Task DeleteBusinessStatisticAsync(List<int> id)
        {
            await BulkDeleteAsync(id);
        }

        public async Task<BusinessStatisticResponse?> GetBusinessFeedbackStatisticsByMonthAsync(int id, int month)
        {
            return await _dbContext.BusinessStatistics
                .AsNoTracking()
                .Where(s => s.BusinessId == id &&
                    s.Month == month &&
                    s.IsMonth == true &&
                    s.Type.Equals(TypeConstants.FEEDBACK))
                .Select(s => new BusinessStatisticResponse
                {
                    BusinessId = s.BusinessId,
                    Type = s.Type,
                    Value = _dbContext.BusinessStatistics
                        .AsNoTracking()
                        .Where(s => s.BusinessId == id &&
                            s.Month == month &&
                            s.IsMonth == true &&
                            s.Type.Equals(TypeConstants.FEEDBACK))
                        .Select(v => new BusinessStatisticValueResponse
                        {
                            Value = v.Value,
                            Date = v.Date,
                            Month = v.Month,
                            Year = v.Year
                        })
                        .ToList(),
                })
                .FirstOrDefaultAsync();
        }

        public async Task<BusinessStatisticResponse?> GetBusinessFeedbackStatisticsByYearAsync(int id)
        {
            return await _dbContext.BusinessStatistics
                .AsNoTracking()
                .Where(s => s.BusinessId == id &&
                    s.Year == DateTime.Now.Year &&
                    s.IsYear == true &&
                    s.Type.Equals(TypeConstants.FEEDBACK))
                .Select(s => new BusinessStatisticResponse
                {
                    BusinessId = s.BusinessId,
                    Type = s.Type,
                    Value = _dbContext.BusinessStatistics
                        .AsNoTracking()
                        .Where(s => s.BusinessId == id &&
                            s.Year == DateTime.Now.Year &&
                            s.IsYear == true &&
                            s.Type.Equals(TypeConstants.FEEDBACK))
                        .Select(v => new BusinessStatisticValueResponse
                        {
                            Value = v.Value,
                            Date = null,
                            Month = v.Month,
                            Year = v.Year
                        })
                        .ToList(),
                })
                .FirstOrDefaultAsync();
        }

        public async Task<decimal?> GetBusinessFeedbackAverageValueByMonthAsync(int id, int month)
        {
            decimal[] value = await _dbContext.BusinessStatistics
                .AsNoTracking()
                .Where(s => s.BusinessId == id &&
                    s.Month == month &&
                    s.IsMonth == true &&
                    s.Type.Equals(TypeConstants.FEEDBACK))
                .Select(s => s.Value)
                .ToArrayAsync();

            return value.Average();
        }

        public async Task<BusinessStatisticResponse?> GetBusinessOrderStatisticsByMonthAsync(int id, int month)
        {
            var values = await _dbContext.BusinessStatistics
                .AsNoTracking()
                .Where(s => s.BusinessId == id &&
                    s.Month == month &&
                    s.IsMonth == true &&
                    s.Type.Equals(TypeConstants.ORDER))
                .Select(v => new BusinessStatisticValueResponse
                {
                    Value = v.Value,
                    Date = v.Date,
                    Month = v.Month,
                    Year = v.Year
                })
                .ToListAsync();

            return await _dbContext.BusinessStatistics
                .AsNoTracking()
                .Where(s => s.BusinessId == id &&
                    s.Month == month &&
                    s.IsMonth == true &&
                    s.Type.Equals(TypeConstants.ORDER))
                .Select(s => new BusinessStatisticResponse
                {
                    BusinessId = s.BusinessId,
                    Type = s.Type,
                    Value = values
                })
                .FirstOrDefaultAsync();
        }

        public async Task<BusinessStatisticResponse?> GetBusinessOrderStatisticsByYearAsync(int id)
        {
            var values = await _dbContext.BusinessStatistics
                .AsNoTracking()
                .Where(s => s.BusinessId == id &&
                    s.Year == DateTime.Now.Year &&
                    s.IsYear == true &&
                    s.Type.Equals(TypeConstants.ORDER))
                .Select(v => new BusinessStatisticValueResponse
                {
                    Value = v.Value,
                    Date = null,
                    Month = v.Month,
                    Year = v.Year
                })
                .ToListAsync();

            return await _dbContext.BusinessStatistics
                .AsNoTracking()
                .Where(s => s.BusinessId == id &&
                    s.Year == DateTime.Now.Year &&
                    s.IsYear == true &&
                    s.Type.Equals(TypeConstants.ORDER))
                .Select(s => new BusinessStatisticResponse
                {
                    BusinessId = s.BusinessId,
                    Type = s.Type,
                    Value = values,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<decimal?> GetBusinessOrderAverageValueByMonthAsync(int id, int month)
        {
            decimal[] value = await _dbContext.BusinessStatistics
                .AsNoTracking()
                .Where(s => s.BusinessId == id &&
                    s.Month == month &&
                    s.IsMonth == true &&
                    s.Type.Equals(TypeConstants.ORDER))
                .Select(s => s.Value)
                .ToArrayAsync();

            return value.Average();
        }

        public async Task<BusinessStatisticResponse?> GetBusinessProfitStatisticsByMonthAsync(int id, int month)
        {
            return await _dbContext.BusinessStatistics
                .AsNoTracking()
                .Where(s => s.BusinessId == id &&
                    s.Month == month &&
                    s.IsMonth == true &&
                    s.Type.Equals(TypeConstants.PROFIT))
                .Select(s => new BusinessStatisticResponse
                {
                    BusinessId = s.BusinessId,
                    Type = s.Type,
                    Value = _dbContext.BusinessStatistics
                        .AsNoTracking()
                        .Where(s => s.BusinessId == id &&
                            s.Month == month &&
                            s.IsMonth == true &&
                            s.Type.Equals(TypeConstants.FEEDBACK))
                        .Select(v => new BusinessStatisticValueResponse
                        {
                            Value = v.Value,
                            Date = v.Date,
                            Month = v.Month,
                            Year = v.Year
                        })
                        .ToList(),
                })
                .FirstOrDefaultAsync();
        }

        public async Task<BusinessStatisticResponse?> GetBusinessProfitStatisticsByYearAsync(int id)
        {
            var values = await _dbContext.BusinessStatistics
                .AsNoTracking()
                .Where(s => s.BusinessId == id &&
                    s.Year == DateTime.Now.Year &&
                    s.IsYear == true &&
                    s.Type.Equals(TypeConstants.PROFIT))
                .Select(v => new BusinessStatisticValueResponse
                {
                    Value = v.Value,
                    Date = null,
                    Month = v.Month,
                    Year = v.Year
                })
                .ToListAsync();

            return await _dbContext.BusinessStatistics
                .AsNoTracking()
                .Where(s => s.BusinessId == id &&
                    s.Year == DateTime.Now.Year &&
                    s.IsYear == true &&
                    s.Type.Equals(TypeConstants.PROFIT))
                .Select(s => new BusinessStatisticResponse
                {
                    BusinessId = s.BusinessId,
                    Type = s.Type,
                    Value = values,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<decimal?> GetBusinessProfitAverageValueByMonthAsync(int id, int month)
        {
            decimal[] value = await _dbContext.BusinessStatistics
                .AsNoTracking()
                .Where(s => s.BusinessId == id &&
                    s.Month == month &&
                    s.IsMonth == true &&
                    s.Type.Equals(TypeConstants.PROFIT))
                .Select(s => s.Value)
                .ToArrayAsync();

            return value.Average();
        }

        public async Task UpdateBusinessStatisticAsync(BusinessStatistic businessStatistic)
        {
            await UpdateAsync(businessStatistic);
        }

        public async Task<int[]?> GetBusinessOrderIdsByMonthAsync(int id, int month)
        {
            return await _dbContext.BusinessStatistics
                .AsNoTracking()
                .Where(s => s.BusinessId == id &&
                    s.Month == month &&
                    s.IsMonth == true &&
                    s.Type.Equals(TypeConstants.ORDER))
                .Select(s => s.Id)
                .ToArrayAsync();
        }

        public async Task<int[]?> GetBusinessProfitIdsByMonthAsync(int id, int month)
        {
            return await _dbContext.BusinessStatistics
               .AsNoTracking()
               .Where(s => s.BusinessId == id &&
                   s.Month == month &&
                   s.IsMonth == true &&
                   s.Type.Equals(TypeConstants.PROFIT))
               .Select(s => s.Id)
               .ToArrayAsync();
        }

        public async Task<int[]?> GetBusinessFeedbackIdsByMonthAsync(int id, int month)
        {
            return await _dbContext.BusinessStatistics
               .AsNoTracking()
               .Where(s => s.BusinessId == id &&
                   s.Month == month &&
                   s.IsMonth == true &&
                   s.Type.Equals(TypeConstants.FEEDBACK))
               .Select(s => s.Id)
               .ToArrayAsync();
        }

        public async Task<int[]?> GetBusinessOrderIdsByYearAsync(int id)
        {
            return await _dbContext.BusinessStatistics
               .AsNoTracking()
               .Where(s => s.BusinessId == id &&
                   s.Year == DateTime.Now.Year &&
                   s.IsYear == true &&
                   s.Type.Equals(TypeConstants.ORDER))
               .Select(s => s.Id)
               .ToArrayAsync();
        }

        public async Task<int[]?> GetBusinessProfitIdsByYearAsync(int id)
        {
            return await _dbContext.BusinessStatistics
                .AsNoTracking()
                .Where(s => s.BusinessId == id &&
                    s.Year == DateTime.Now.Year &&
                    s.IsYear == true &&
                    s.Type.Equals(TypeConstants.PROFIT))
                .Select(s => s.Id)
                .ToArrayAsync();
        }

        public async Task<int[]?> GetBusinessFeedbackIdsByYearAsync(int id)
        {
            return await _dbContext.BusinessStatistics
                .AsNoTracking()
                .Where(s => s.BusinessId == id &&
                    s.Year == DateTime.Now.Year &&
                    s.IsYear == true &&
                    s.Type.Equals(TypeConstants.FEEDBACK))
                .Select(s => s.Id)
                .ToArrayAsync();
        }
    }
}
