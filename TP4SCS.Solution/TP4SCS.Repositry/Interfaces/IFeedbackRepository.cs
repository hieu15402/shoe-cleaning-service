using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;

namespace TP4SCS.Repository.Interfaces
{
    public interface IFeedbackRepository : IGenericRepository<Feedback>
    {
        Task AddFeedbacksAsync(Feedback feedback);

        Task<IEnumerable<Feedback>?> GetFeedbacksAsync(
            string? status = null,
            int? pageIndex = null,
            int? pageSize = null,
            OrderByEnum orderBy = OrderByEnum.IdDesc);

        Task<IEnumerable<Feedback>?> GetFeedbacksByServiceIdAsync(
            int serviceId,
            string? status = null,
            int? pageIndex = null,
            int? pageSize = null,
            OrderByEnum orderBy = OrderByEnum.IdAsc);
        Task DeleteFeedbackAsync(int id);

        Task UpdateFeedbackAsync(Feedback feedback);

        Task<Feedback?> GetFeedbackByidAsync(int id);

        Task<IEnumerable<Feedback>?> GetFeedbacksByAccountIdAsync(
           int accountId,
           string? status = null,
           int? pageIndex = null,
           int? pageSize = null,
           OrderByEnum orderBy = OrderByEnum.IdAsc);

        Task<IEnumerable<Feedback>?> GetFeedbacksByBranchIdIdAsync(
            int branchId,
            string? status = null,
            OrderByEnum orderBy = OrderByEnum.IdDesc);

        Task<IEnumerable<Feedback>?> GetFeedbacksByBusinessIdIdAsync(
            int businessId,
            string? status = null,
            OrderByEnum orderBy = OrderByEnum.IdDesc);

        Task<decimal> GetMonthAverageRatingByBusinessIdAsync(int id);

        Task<decimal> GetYearAverageRatingByBusinessIdAsync(int id);

        Task<(decimal TotalRating, int RatingCount)> GetAverageRatingByBusinessIdAsync(int id);

        Task<Dictionary<int, decimal>> GetMonthAverageRatingsByBusinessIdAsync(int id);

        Task<Dictionary<int, decimal>> GetYearAverageRatingsByBusinessIdAsync(int id);
    }
}