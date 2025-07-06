using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Response.BusinessStatistic;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Repository.Interfaces;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class BusinessStatisticService : IBusinessStatisticService
    {
        private readonly IBusinessStatisticRepository _businessStatisticRepository;
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IOrderRepository _orderRepository;

        public BusinessStatisticService(IBusinessStatisticRepository businessStatisticRepository,
            IFeedbackRepository feedbackRepository,
            IOrderRepository orderRepository)
        {
            _businessStatisticRepository = businessStatisticRepository;
            _feedbackRepository = feedbackRepository;
            _orderRepository = orderRepository;
        }

        public async Task<ApiResponse<BusinessStatisticResponse>> GetBusinessFeedbackStatisticByMonthAsync(int id)
        {
            var statistic = await _businessStatisticRepository.GetBusinessFeedbackStatisticsByMonthAsync(id, DateTime.Now.Month);

            if (statistic == null)
            {
                var result = await UpdateBusinessFeedbackStatisticAsync(id);

                if (result.StatusCode != 200)
                {
                    return new ApiResponse<BusinessStatisticResponse>("error", 400, "Lấy Thông Tin Thống Kê Thất Bại!");
                }

                statistic = await _businessStatisticRepository.GetBusinessFeedbackStatisticsByMonthAsync(id, DateTime.Now.Month);
            }

            return new ApiResponse<BusinessStatisticResponse>("success", "Lấy Thông Tin Thống Kê Thành Công!", statistic);
        }

        public async Task<ApiResponse<BusinessStatisticResponse>> GetBusinessFeedbackStatisticByYearAsync(int id)
        {
            var statistic = await _businessStatisticRepository.GetBusinessFeedbackStatisticsByYearAsync(id);

            if (statistic == null)
            {
                var result = await UpdateBusinessFeedbackStatisticAsync(id);

                if (result.StatusCode != 200)
                {
                    return new ApiResponse<BusinessStatisticResponse>("error", 400, "Lấy Thông Tin Thống Kê Thất Bại!");
                }

                statistic = await _businessStatisticRepository.GetBusinessFeedbackStatisticsByYearAsync(id);
            }

            return new ApiResponse<BusinessStatisticResponse>("success", "Lấy Thông Tin Thống Kê Thành Công!", statistic);
        }

        public async Task<ApiResponse<BusinessStatisticResponse>> GetBusinessOrderStatisticByMonthAsync(int id)
        {
            var statistic = await _businessStatisticRepository.GetBusinessOrderStatisticsByMonthAsync(id, DateTime.Now.Month);

            if (statistic == null)
            {
                var result = await UpdateBusinessOrderStatisticAsync(id);

                if (result.StatusCode != 200)
                {
                    return new ApiResponse<BusinessStatisticResponse>("error", 400, "Lấy Thông Tin Thống Kê Thất Bại!");
                }

                statistic = await _businessStatisticRepository.GetBusinessOrderStatisticsByMonthAsync(id, DateTime.Now.Month);
            }

            return new ApiResponse<BusinessStatisticResponse>("success", "Lấy Thông Tin Thống Kê Thành Công!", statistic);
        }

        public async Task<ApiResponse<BusinessStatisticResponse>> GetBusinessOrderStatisticByYearAsync(int id)
        {
            var statistic = await _businessStatisticRepository.GetBusinessOrderStatisticsByYearAsync(id);

            if (statistic == null)
            {
                var result = await UpdateBusinessOrderStatisticAsync(id);

                if (result.StatusCode != 200)
                {
                    return new ApiResponse<BusinessStatisticResponse>("error", 400, "Lấy Thông Tin Thống Kê Thất Bại!");
                }

                statistic = await _businessStatisticRepository.GetBusinessOrderStatisticsByYearAsync(id);
            }

            return new ApiResponse<BusinessStatisticResponse>("success", "Lấy Thông Tin Thống Kê Thành Công!", statistic);
        }

        public async Task<ApiResponse<BusinessStatisticResponse>> GetBusinessProfitStatisticByMonthAsync(int id)
        {
            var statistic = await _businessStatisticRepository.GetBusinessProfitStatisticsByMonthAsync(id, DateTime.Now.Month);

            if (statistic == null)
            {
                var result = await UpdateBusinessProfitStatisticAsync(id);

                if (result.StatusCode != 200)
                {
                    return new ApiResponse<BusinessStatisticResponse>("error", 400, "Lấy Thông Tin Thống Kê Thất Bại!");
                }

                statistic = await _businessStatisticRepository.GetBusinessProfitStatisticsByMonthAsync(id, DateTime.Now.Month);
            }

            return new ApiResponse<BusinessStatisticResponse>("success", "Lấy Thông Tin Thống Kê Thành Công!", statistic);
        }

        public async Task<ApiResponse<BusinessStatisticResponse>> GetBusinessProfitStatisticByYearAsync(int id)
        {
            var statistic = await _businessStatisticRepository.GetBusinessProfitStatisticsByYearAsync(id);

            if (statistic == null)
            {
                var result = await UpdateBusinessProfitStatisticAsync(id);

                if (result.StatusCode != 200)
                {
                    return new ApiResponse<BusinessStatisticResponse>("error", 400, "Lấy Thông Tin Thống Kê Thất Bại!");
                }

                statistic = await _businessStatisticRepository.GetBusinessProfitStatisticsByYearAsync(id);
            }

            return new ApiResponse<BusinessStatisticResponse>("success", "Lấy Thông Tin Thống Kê Thành Công!", statistic);
        }

        public async Task<ApiResponse<BusinessStatisticResponse>> UpdateBusinessFeedbackStatisticAsync(int id)
        {
            var monthIds = await _businessStatisticRepository.GetBusinessFeedbackIdsByMonthAsync(id, DateTime.Now.Month);

            var yearIds = await _businessStatisticRepository.GetBusinessFeedbackIdsByYearAsync(id);

            if (monthIds!.Length != 0)
            {
                await _businessStatisticRepository.DeleteBusinessStatisticAsync(monthIds.ToList());
            }

            if (yearIds!.Length != 0)
            {
                await _businessStatisticRepository.DeleteBusinessStatisticAsync(yearIds.ToList());
            }

            var monthStatistic = await _feedbackRepository.GetMonthAverageRatingsByBusinessIdAsync(id);
            var yearStatistic = await _feedbackRepository.GetYearAverageRatingsByBusinessIdAsync(id);

            List<BusinessStatistic> monthList = new List<BusinessStatistic>();
            List<BusinessStatistic> yearList = new List<BusinessStatistic>();

            try
            {
                foreach (var item in monthStatistic)
                {
                    var day = item.Key;
                    var value = item.Value;

                    var newMonthStatistic = new BusinessStatistic();
                    newMonthStatistic.BusinessId = id;
                    newMonthStatistic.Month = DateTime.Now.Month;
                    newMonthStatistic.Year = DateTime.Now.Year;
                    newMonthStatistic.IsMonth = true;
                    newMonthStatistic.IsYear = false;
                    newMonthStatistic.Type = TypeConstants.FEEDBACK;
                    newMonthStatistic.Date = day;
                    newMonthStatistic.Value = value;

                    monthList.Add(newMonthStatistic);
                }

                await _businessStatisticRepository.CreateBusinessStatisticAsync(monthList);

                foreach (var item in yearStatistic)
                {
                    var month = item.Key;
                    var value = item.Value;

                    var newYearStatistic = new BusinessStatistic();
                    newYearStatistic.BusinessId = id;
                    newYearStatistic.Date = 0;
                    newYearStatistic.Year = DateTime.Now.Year;
                    newYearStatistic.IsMonth = false;
                    newYearStatistic.IsYear = true;
                    newYearStatistic.Type = TypeConstants.FEEDBACK;
                    newYearStatistic.Month = month;
                    newYearStatistic.Value = value;

                    yearList.Add(newYearStatistic);
                }

                await _businessStatisticRepository.CreateBusinessStatisticAsync(yearList);

                return new ApiResponse<BusinessStatisticResponse>("success", "Cập Nhập Thống Kê Đánh Giá Thành Công!", null);
            }
            catch (Exception)
            {
                return new ApiResponse<BusinessStatisticResponse>("error", 400, "Cập Nhập Thống Kê Đánh Giá Thất Bại!");
            }
        }

        public async Task<ApiResponse<BusinessStatisticResponse>> UpdateBusinessOrderStatisticAsync(int id)
        {
            var monthIds = await _businessStatisticRepository.GetBusinessOrderIdsByMonthAsync(id, DateTime.Now.Month);

            var yearIds = await _businessStatisticRepository.GetBusinessOrderIdsByYearAsync(id);

            if (monthIds!.Length != 0)
            {
                await _businessStatisticRepository.DeleteBusinessStatisticAsync(monthIds.ToList());
            }

            if (yearIds!.Length != 0)
            {
                await _businessStatisticRepository.DeleteBusinessStatisticAsync(yearIds.ToList());
            }

            var monthStatistic = await _orderRepository.CountMonthOrdersByBusinessIdAsync(id);
            var yearStatistic = await _orderRepository.CountYearOrdersByBusinessIdAsync(id);

            List<BusinessStatistic> monthList = new List<BusinessStatistic>();
            List<BusinessStatistic> yearList = new List<BusinessStatistic>();

            try
            {
                foreach (var item in monthStatistic)
                {
                    var day = item.Key;
                    var value = item.Value;

                    var newMonthStatistic = new BusinessStatistic();
                    newMonthStatistic.BusinessId = id;
                    newMonthStatistic.Month = DateTime.Now.Month;
                    newMonthStatistic.Year = DateTime.Now.Year;
                    newMonthStatistic.IsMonth = true;
                    newMonthStatistic.IsYear = false;
                    newMonthStatistic.Type = TypeConstants.ORDER;
                    newMonthStatistic.Date = day;
                    newMonthStatistic.Value = value;

                    monthList.Add(newMonthStatistic);
                }

                await _businessStatisticRepository.CreateBusinessStatisticAsync(monthList);

                foreach (var item in yearStatistic)
                {
                    var month = item.Key;
                    var value = item.Value;

                    var newYearStatistic = new BusinessStatistic();
                    newYearStatistic.BusinessId = id;
                    newYearStatistic.Date = 0;
                    newYearStatistic.Year = DateTime.Now.Year;
                    newYearStatistic.IsMonth = false;
                    newYearStatistic.IsYear = true;
                    newYearStatistic.Type = TypeConstants.ORDER;
                    newYearStatistic.Month = month;
                    newYearStatistic.Value = value;

                    yearList.Add(newYearStatistic);
                }

                await _businessStatisticRepository.CreateBusinessStatisticAsync(yearList);

                return new ApiResponse<BusinessStatisticResponse>("success", "Cập Nhập Thống Kê Đánh Giá Thành Công!", null);
            }
            catch (Exception)
            {
                return new ApiResponse<BusinessStatisticResponse>("error", 400, "Cập Nhập Thống Kê Đánh Giá Thất Bại!");
            }
        }

        public async Task<ApiResponse<BusinessStatisticResponse>> UpdateBusinessProfitStatisticAsync(int id)
        {
            var monthIds = await _businessStatisticRepository.GetBusinessProfitIdsByMonthAsync(id, DateTime.Now.Month);

            var yearIds = await _businessStatisticRepository.GetBusinessProfitIdsByYearAsync(id);

            if (monthIds!.Length != 0)
            {
                await _businessStatisticRepository.DeleteBusinessStatisticAsync(monthIds.ToList());
            }

            if (yearIds!.Length != 0)
            {
                await _businessStatisticRepository.DeleteBusinessStatisticAsync(yearIds.ToList());
            }

            var monthStatistic = await _orderRepository.SumMonthOrderProfitByBusinessIdAsync(id);
            var yearStatistic = await _orderRepository.SumYearOrderProfitByBusinessIdAsync(id);

            List<BusinessStatistic> monthList = new List<BusinessStatistic>();
            List<BusinessStatistic> yearList = new List<BusinessStatistic>();

            try
            {
                foreach (var item in monthStatistic)
                {
                    var day = item.Key;
                    var value = item.Value;

                    var newMonthStatistic = new BusinessStatistic();
                    newMonthStatistic.BusinessId = id;
                    newMonthStatistic.Month = DateTime.Now.Month;
                    newMonthStatistic.Year = DateTime.Now.Year;
                    newMonthStatistic.IsMonth = true;
                    newMonthStatistic.IsYear = false;
                    newMonthStatistic.Type = TypeConstants.PROFIT;
                    newMonthStatistic.Date = day;
                    newMonthStatistic.Value = value;

                    monthList.Add(newMonthStatistic);
                }

                await _businessStatisticRepository.CreateBusinessStatisticAsync(monthList);

                foreach (var item in yearStatistic)
                {
                    var month = item.Key;
                    var value = item.Value;

                    var newYearStatistic = new BusinessStatistic();
                    newYearStatistic.BusinessId = id;
                    newYearStatistic.Date = 0;
                    newYearStatistic.Year = DateTime.Now.Year;
                    newYearStatistic.IsMonth = false;
                    newYearStatistic.IsYear = true;
                    newYearStatistic.Type = TypeConstants.PROFIT;
                    newYearStatistic.Month = month;
                    newYearStatistic.Value = value;

                    yearList.Add(newYearStatistic);
                }

                await _businessStatisticRepository.CreateBusinessStatisticAsync(yearList);

                return new ApiResponse<BusinessStatisticResponse>("success", "Cập Nhập Thống Kê Đánh Giá Thành Công!", null);
            }
            catch (Exception)
            {
                return new ApiResponse<BusinessStatisticResponse>("error", 400, "Cập Nhập Thống Kê Đánh Giá Thất Bại!");
            }
        }
    }
}
