using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.Leaderboard;
using TP4SCS.Repository.Interfaces;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class LeaderboardService : ILeaderboardService
    {
        private readonly ILeaderboardRepository _leaderboardRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IOrderRepository _orderRepository;

        public LeaderboardService(ILeaderboardRepository leaderboardRepository,
            IBusinessRepository businessRepository,
            IFeedbackRepository feedbackRepository,
            IOrderRepository orderRepository)
        {
            _leaderboardRepository = leaderboardRepository;
            _businessRepository = businessRepository;
            _feedbackRepository = feedbackRepository;
            _orderRepository = orderRepository;
        }

        public async Task<ApiResponse<LeaderboardResponse?>> GetLeaderboardByMonthAsync()
        {
            try
            {
                var board = await _leaderboardRepository.GetLeaderboardByMonthAsync();

                if (board != null)
                {
                    return new ApiResponse<LeaderboardResponse?>("sucess", "Lấy Thông Tin Bảng Xếp Hạng Thành Công!", board);
                }
                else
                {
                    var result = await UpdateLeaderboardAsync();

                    if (result.StatusCode == 200)
                    {
                        board = await _leaderboardRepository.GetLeaderboardByMonthAsync();

                        return new ApiResponse<LeaderboardResponse?>("sucess", "Lấy Thông Tin Bảng Xếp Hạng Thành Công!", board);
                    }
                    else
                    {
                        return new ApiResponse<LeaderboardResponse?>("error", 400, "Lấy Thông Tin Bảng Xếp Hạng Thất Bại!");
                    }
                }
            }
            catch (Exception)
            {
                return new ApiResponse<LeaderboardResponse?>("error", 400, "Lấy Thông Tin Bảng Xếp Hạng Thất Bại!");
            }
        }

        public async Task<ApiResponse<LeaderboardResponse?>> GetLeaderboardByYearAsync()
        {
            try
            {
                var board = await _leaderboardRepository.GetLeaderboardByYearAsync();

                if (board != null)
                {
                    return new ApiResponse<LeaderboardResponse?>("sucess", "Lấy Thông Tin Bảng Xếp Hạng Thành Công!", board);
                }
                else
                {
                    var result = await UpdateLeaderboardAsync();

                    if (result.StatusCode == 200)
                    {
                        board = await _leaderboardRepository.GetLeaderboardByYearAsync();

                        return new ApiResponse<LeaderboardResponse?>("sucess", "Lấy Thông Tin Bảng Xếp Hạng Thành Công!", board);
                    }
                    else
                    {
                        return new ApiResponse<LeaderboardResponse?>("error", 400, "Lấy Thông Tin Bảng Xếp Hạng Thất Bại!");
                    }
                }
            }
            catch (Exception)
            {
                return new ApiResponse<LeaderboardResponse?>("error", 400, "Lấy Thông Tin Bảng Xếp Hạng Thất Bại!");
            }
        }

        public async Task<ApiResponse<LeaderboardResponse>> UpdateLeaderboardAsync()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var monthBoard = await _leaderboardRepository.GetLeaderboardByMonthAsync();

            var yearBoard = await _leaderboardRepository.GetLeaderboardByYearNoTrackingAsync();

            if (monthBoard != null)
            {
                await _leaderboardRepository.DeleteLeaderboardAsync(monthBoard.Id);
            }

            if (yearBoard != null)
            {
                await _leaderboardRepository.DeleteLeaderboardAsync(yearBoard.Id);
            }

            Dictionary<int, decimal> monthDictionary = new Dictionary<int, decimal>();
            Dictionary<int, decimal> yearDictionary = new Dictionary<int, decimal>();

            var businessIds = await _businessRepository.GetBusinessIdsAsync();

            foreach (var businessId in businessIds!)
            {
                var monthRating = await _feedbackRepository.GetMonthAverageRatingByBusinessIdAsync(businessId);
                var yearRating = await _feedbackRepository.GetYearAverageRatingByBusinessIdAsync(businessId);

                var monthOrderNum = await _orderRepository.CountMonthOrderByBusinessIdAsync(businessId);
                var yearOrderNum = await _orderRepository.CountYearOrderByBusinessIdAsync(businessId);

                var monthValue = monthRating * (1 + 0.3m + (decimal)Math.Sqrt((double)monthOrderNum));
                var yearValue = yearRating * (1 + 0.3m + (decimal)Math.Sqrt((double)yearOrderNum));

                monthDictionary.Add(businessId, monthValue);
                yearDictionary.Add(businessId, yearValue);
            }

            monthDictionary = monthDictionary.OrderByDescending(x => x.Value).Take(10).ToDictionary(x => x.Key, x => x.Value);
            yearDictionary = yearDictionary.OrderByDescending(x => x.Value).Take(10).ToDictionary(x => x.Key, x => x.Value);

            List<int> topMonthBusinessIds = monthDictionary.Keys.ToList();
            List<int> topYearBusinessIds = yearDictionary.Keys.ToList();

            Leaderboard newMonthBoard = new Leaderboard();
            newMonthBoard.BusinessIds = string.Join(",", topMonthBusinessIds);
            newMonthBoard.Month = currentMonth;
            newMonthBoard.Year = currentYear;
            newMonthBoard.IsMonth = true;
            newMonthBoard.IsYear = false;

            Leaderboard newYearBoard = new Leaderboard();
            newYearBoard.BusinessIds = string.Join(",", topYearBusinessIds);
            newYearBoard.Month = currentMonth;
            newYearBoard.Year = currentYear;
            newYearBoard.IsMonth = false;
            newYearBoard.IsYear = true;

            try
            {
                await _leaderboardRepository.RunInTransactionAsync(async () =>
                {
                    await _leaderboardRepository.CreateLeaderboardAsync(newMonthBoard);

                    await _leaderboardRepository.CreateLeaderboardAsync(newYearBoard);
                });

                return new ApiResponse<LeaderboardResponse>("success", "Cập Nhập Xếp Hạng Thành Công", null);
            }
            catch (Exception)
            {
                return new ApiResponse<LeaderboardResponse>("error", 400, "Cập Nhập Xếp Hạng Thất Bại!");
            }
        }
    }
}
