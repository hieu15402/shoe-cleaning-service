using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Repository.Interfaces;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IOpenAIService _openAIService;
        private readonly IBusinessService _businessService;

        public FeedbackService(IFeedbackRepository feedbackRepository, IOpenAIService openAIService, IBusinessService businessService)
        {
            _feedbackRepository = feedbackRepository;
            _openAIService = openAIService;
            _businessService = businessService;
        }

        public async Task<IEnumerable<Feedback>?> GetFeedbacks(string? status, OrderByEnum order)
        {
            return await _feedbackRepository.GetFeedbacksAsync(status, null, null, order);
        }
        public async Task<Feedback?> GetFeedbackByIdAsync(int id)
        {
            return await _feedbackRepository.GetFeedbackByidAsync(id);
        }

        public async Task<IEnumerable<Feedback>?> GetFeedbackByServiceId(int serviceId)
        {
            return await _feedbackRepository.GetFeedbacksByServiceIdAsync(serviceId,null,null,null,OrderByEnum.IdDesc);
        }

        public async Task<IEnumerable<Feedback>?> GetFeedbackByAccountId(int accountId)
        {
            return await _feedbackRepository.GetFeedbacksByAccountIdAsync(accountId);
        }

        public async Task<(IEnumerable<Feedback> Feedbacks, int TotalCount)> GetFeedbackByBranchIdIdAsync(
            int branchId,
            int pageIndex = 1,
            int pageSize = 10)
        {
            // Lấy toàn bộ feedbacks theo branchId
            var feedbacks = await _feedbackRepository.GetFeedbacksByBranchIdIdAsync(branchId);

            // Tính tổng số feedbacks trước khi phân trang
            int totalCount = feedbacks?.Count() ?? 0;

            // Áp dụng phân trang hoặc trả về danh sách trống nếu feedbacks là null
            var pagedFeedbacks = feedbacks?
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList() ?? new List<Feedback>();

            // Trả về danh sách phân trang và tổng số feedbacks
            return (pagedFeedbacks, totalCount);
        }

        public async Task<(IEnumerable<Feedback> Feedbacks, int TotalCount)> GetFeedbackByBusinessIdIdAsync(
            int businessId,
            int pageIndex = 1,
            int pageSize = 10)
        {
            var feedbacks = await _feedbackRepository.GetFeedbacksByBusinessIdIdAsync(businessId);

            // Tính tổng số feedbacks trước khi phân trang
            int totalCount = feedbacks?.Count() ?? 0;

            // Áp dụng phân trang hoặc trả về danh sách trống nếu feedbacks là null
            var pagedFeedbacks = feedbacks?
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList() ?? new List<Feedback>();

            // Trả về danh sách phân trang và tổng số feedbacks
            return (pagedFeedbacks, totalCount);
        }
        public async Task AddFeedbacksAsync(HttpClient httpClient, Feedback feedback)
        {
            if (feedback.Rating < 0 || feedback.Rating > 5)
            {
                throw new ArgumentOutOfRangeException(nameof(feedback.Rating), "Rating phải nằm trong khoảng từ 0 đến 5.");
            }

            if (feedback.Content != null && feedback.Content.Length > 500)
            {
                throw new ArgumentException("Nội dung feedback không được vượt quá 500 ký tự.", nameof(feedback.Content));
            }
            await _businessService.UpdateBusinessRatingAsync(await _businessService.GetBusinessIdByOrderItemIdAsync(feedback.OrderItemId), feedback.Rating);
            feedback.IsValidAsset = true;
            feedback.Status = StatusConstants.PENDING;
            feedback.CreatedTime = DateTime.Now;
            feedback.IsAllowedUpdate = true;
            if (!string.IsNullOrEmpty(feedback.Content))
            {
                var isValidContent = await _openAIService.ValidateFeedbackContentAsync(httpClient, feedback.Content);

                feedback.IsValidContent = isValidContent;
            }
            else
            {
                feedback.IsValidContent = true;
            }

            await _feedbackRepository.AddFeedbacksAsync(feedback);
        }

        public async Task DeleteFeedbackAsync(int id)
        {
            await _feedbackRepository.DeleteFeedbackAsync(id);
        }

        public async Task UpdateFeedbackAsync(bool? isValidAsset, bool? IsValidContent, string? status, int existingFeedbackId)
        {
            var existingFeedback = await _feedbackRepository.GetFeedbackByidAsync(existingFeedbackId);
            if (existingFeedback == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy đánh giá với ID: {existingFeedbackId}.");
            }
            if (status != null)
            {
                existingFeedback.Status = status;
            }
            if (IsValidContent.HasValue)
            {
                existingFeedback.IsValidContent = IsValidContent.Value;
            }
            if (isValidAsset.HasValue)
            {
                existingFeedback.IsValidAsset = isValidAsset.Value;
            }

            await _feedbackRepository.UpdateFeedbackAsync(existingFeedback);
        }

        public async Task UpdateContentFeedbackAsync(Feedback feedback, int existingFeedbackId, HttpClient httpClient)
        {
            var existingFeedback = await _feedbackRepository.GetFeedbackByidAsync(existingFeedbackId);
            if (existingFeedback == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy đánh giá với ID: {existingFeedbackId}.");
            }
            existingFeedback.Content = feedback.Content;
            existingFeedback.AssetUrls = feedback.AssetUrls;
            if (!string.IsNullOrEmpty(feedback.Content))
            {
                var isValidContent = await _openAIService.ValidateFeedbackContentAsync(httpClient, feedback.Content);

                existingFeedback.IsValidContent = isValidContent;
            }
            else
            {
                feedback.IsValidContent = true;
            }
            existingFeedback.IsAllowedUpdate = false;
            existingFeedback.Status = StatusConstants.PENDING;
            existingFeedback.IsValidAsset = true;
            await _feedbackRepository.UpdateFeedbackAsync(existingFeedback);
        }

        public async Task ReplyFeedbackAsync(string reply, int existingFeedbackId)
        {
            var existingFeedback = await _feedbackRepository.GetFeedbackByidAsync(existingFeedbackId);
            if (existingFeedback == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy đánh giá với ID: {existingFeedbackId}.");
            }
            existingFeedback.Reply = reply;

            await _feedbackRepository.UpdateFeedbackAsync(existingFeedback);
        }

        public async Task UpdateFeedbackAsync(string replyComment, int existingFeedbackId)
        {
            var existingFeedback = await _feedbackRepository.GetFeedbackByidAsync(existingFeedbackId);
            if (existingFeedback == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy đánh giá với ID: {existingFeedbackId}.");
            }

            await _feedbackRepository.UpdateFeedbackAsync(existingFeedback);
        }
    }
}
