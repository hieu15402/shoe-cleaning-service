using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Response.BusinessStatistic;

namespace TP4SCS.Repository.Interfaces
{
    public interface IBusinessStatisticRepository : IGenericRepository<BusinessStatistic>
    {
        Task<BusinessStatisticResponse?> GetBusinessOrderStatisticsByMonthAsync(int id, int month);

        Task<BusinessStatisticResponse?> GetBusinessProfitStatisticsByMonthAsync(int id, int month);

        Task<BusinessStatisticResponse?> GetBusinessFeedbackStatisticsByMonthAsync(int id, int month);

        Task<decimal?> GetBusinessOrderAverageValueByMonthAsync(int id, int month);

        Task<decimal?> GetBusinessProfitAverageValueByMonthAsync(int id, int month);

        Task<decimal?> GetBusinessFeedbackAverageValueByMonthAsync(int id, int month);

        Task<int[]?> GetBusinessOrderIdsByMonthAsync(int id, int month);

        Task<int[]?> GetBusinessProfitIdsByMonthAsync(int id, int month);

        Task<int[]?> GetBusinessFeedbackIdsByMonthAsync(int id, int month);

        Task<BusinessStatisticResponse?> GetBusinessOrderStatisticsByYearAsync(int id);

        Task<BusinessStatisticResponse?> GetBusinessProfitStatisticsByYearAsync(int id);

        Task<BusinessStatisticResponse?> GetBusinessFeedbackStatisticsByYearAsync(int id);

        Task<int[]?> GetBusinessOrderIdsByYearAsync(int id);

        Task<int[]?> GetBusinessProfitIdsByYearAsync(int id);

        Task<int[]?> GetBusinessFeedbackIdsByYearAsync(int id);

        Task CreateBusinessStatisticAsync(List<BusinessStatistic> businessStatistic);

        Task DeleteBusinessStatisticAsync(List<int> id);
    }
}
