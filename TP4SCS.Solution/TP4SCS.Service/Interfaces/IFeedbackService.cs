using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;

namespace TP4SCS.Services.Interfaces
{
    public interface IFeedbackService
    {
        Task<IEnumerable<Feedback>?> GetFeedbacks(string? status, OrderByEnum order);

        Task AddFeedbacksAsync(HttpClient httpClient, Feedback feedback);

        Task<IEnumerable<Feedback>?> GetFeedbackByServiceId(int serviceId);
        Task<Feedback?> GetFeedbackByIdAsync(int id);

        Task DeleteFeedbackAsync(int id);

        Task UpdateFeedbackAsync(bool? isValidAsset, bool? IsValidContent, string? status, int existingFeedbackId);

        Task<IEnumerable<Feedback>?> GetFeedbackByAccountId(int accountId);
        Task<(IEnumerable<Feedback> Feedbacks, int TotalCount)> GetFeedbackByBranchIdIdAsync(
            int branchId,
            int pageIndex = 1,
            int pageSize = 10);
        Task<(IEnumerable<Feedback> Feedbacks, int TotalCount)> GetFeedbackByBusinessIdIdAsync(
            int businessId,
            int pageIndex = 1,
            int pageSize = 10);
        Task ReplyFeedbackAsync(string reply, int existingFeedbackId);
        Task UpdateContentFeedbackAsync(Feedback feedback, int existingFeedbackId, HttpClient httpClient);
    }
}
