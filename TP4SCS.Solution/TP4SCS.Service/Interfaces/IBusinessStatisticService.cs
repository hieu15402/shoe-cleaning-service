using TP4SCS.Library.Models.Response.BusinessStatistic;
using TP4SCS.Library.Models.Response.General;

namespace TP4SCS.Services.Interfaces
{
    public interface IBusinessStatisticService
    {
        Task<ApiResponse<BusinessStatisticResponse>> GetBusinessOrderStatisticByMonthAsync(int id);

        Task<ApiResponse<BusinessStatisticResponse>> GetBusinessProfitStatisticByMonthAsync(int id);

        Task<ApiResponse<BusinessStatisticResponse>> GetBusinessFeedbackStatisticByMonthAsync(int id);

        Task<ApiResponse<BusinessStatisticResponse>> GetBusinessOrderStatisticByYearAsync(int id);

        Task<ApiResponse<BusinessStatisticResponse>> GetBusinessProfitStatisticByYearAsync(int id);

        Task<ApiResponse<BusinessStatisticResponse>> GetBusinessFeedbackStatisticByYearAsync(int id);

        Task<ApiResponse<BusinessStatisticResponse>> UpdateBusinessOrderStatisticAsync(int id);

        Task<ApiResponse<BusinessStatisticResponse>> UpdateBusinessProfitStatisticAsync(int id);

        Task<ApiResponse<BusinessStatisticResponse>> UpdateBusinessFeedbackStatisticAsync(int id);
    }
}
