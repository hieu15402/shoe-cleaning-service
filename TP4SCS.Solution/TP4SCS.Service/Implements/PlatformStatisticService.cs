using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.PlatformStatistic;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.PlatformStatistic;
using TP4SCS.Library.Repositories;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Repository.Interfaces;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class PlatformStatisticService : IPlatformStatisticService
    {
        private readonly IPlatformStatisticRepository _platformStatisticRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ITransactionRepository _transactionRepository;

        public PlatformStatisticService(IPlatformStatisticRepository platformStatisticRepository,
            IAccountRepository accountRepository,
            IOrderRepository orderRepository,
            ITransactionRepository transactionRepository)
        {
            _platformStatisticRepository = platformStatisticRepository;
            _accountRepository = accountRepository;
            _orderRepository = orderRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<ApiResponse<PlatformStatisticResponse>> GetPlatformStatisticAsync(GetPlatformStatisticRequest getPlatformStatisticRequest)
        {
            var statistics = await _platformStatisticRepository.GetPlatformStatisticAsync(getPlatformStatisticRequest);

            if (statistics == null)
            {
                var update = new UpdatePlatformStatisticRequest
                {
                    Type = getPlatformStatisticRequest.Type switch
                    {
                        StatisticOption.ORDER => UpdateStatisticOption.ORDER,
                        _ => UpdateStatisticOption.PROFIT
                    },
                };

                var result = await UpdatePlatformStatisticAsync(update);

                if (result.StatusCode != 200)
                {
                    return new ApiResponse<PlatformStatisticResponse>("error", 400, "Không Tìm Thấy Thông Tin Thống Kê!");
                }
            }

            return new ApiResponse<PlatformStatisticResponse>("success", "Lấy Thông Tin Thống Kê Thành Công!", statistics);
        }

        public async Task<ApiResponse<PlatformStatisticResponse>> GetPlatformUserStatisticAsync()
        {
            var statistics = await _platformStatisticRepository.GetPlatformUserStatisticAsync();

            if (statistics == null)
            {
                var update = new UpdatePlatformStatisticRequest
                {
                    Type = UpdateStatisticOption.USER,
                };

                var result = await UpdatePlatformStatisticAsync(update);

                if (result.StatusCode != 200)
                {
                    return new ApiResponse<PlatformStatisticResponse>("error", 400, "Không Tìm Thấy Thông Tin Thống Kê!");
                }
            }

            return new ApiResponse<PlatformStatisticResponse>("success", "Lấy Thông Tin Thống Kê Thành Công!", statistics);
        }

        public async Task<ApiResponse<PlatformStatisticResponse>> UpdatePlatformStatisticAsync(UpdatePlatformStatisticRequest updatePlatformStatisticRequest)
        {
            try
            {
                int currentMonth = DateTime.Now.Month;
                int currentYear = DateTime.Now.Year;

                switch (updatePlatformStatisticRequest.Type)
                {
                    case UpdateStatisticOption.ORDER:

                        var monthOrderIds = await _platformStatisticRepository.GetPlatformOrderStatisticIdsByMonthAsync();

                        var yearOrderIds = await _platformStatisticRepository.GetPlatformOrderStatisticIdsByYearAsync();

                        if (monthOrderIds!.Length != 0)
                        {
                            await _platformStatisticRepository.DeletePlatformStatisticAsync(monthOrderIds.ToList());
                        }

                        if (yearOrderIds!.Length != 0)
                        {
                            await _platformStatisticRepository.DeletePlatformStatisticAsync(yearOrderIds.ToList());
                        }

                        var monthOrderStatistic = await _orderRepository.CountMonthOrdersAsync();
                        var yearOrderStatistic = await _orderRepository.CountYearOrdersAsync();

                        List<PlatformStatistic> monthOrderList = new List<PlatformStatistic>();
                        List<PlatformStatistic> yearOrderList = new List<PlatformStatistic>();

                        foreach (var item in monthOrderStatistic)
                        {
                            var day = item.Key;
                            var value = item.Value;

                            var newMonthStatistic = new PlatformStatistic();
                            newMonthStatistic.Month = DateTime.Now.Month;
                            newMonthStatistic.Year = DateTime.Now.Year;
                            newMonthStatistic.IsMonth = true;
                            newMonthStatistic.IsYear = false;
                            newMonthStatistic.Type = TypeConstants.ORDER;
                            newMonthStatistic.Date = day;
                            newMonthStatistic.Value = value;

                            monthOrderList.Add(newMonthStatistic);
                        }

                        await _platformStatisticRepository.CreatePlatformStatisticAsync(monthOrderList);

                        foreach (var item in yearOrderStatistic)
                        {
                            var month = item.Key;
                            var value = item.Value;

                            var newYearStatistic = new PlatformStatistic();
                            newYearStatistic.Date = 0;
                            newYearStatistic.Year = DateTime.Now.Year;
                            newYearStatistic.IsMonth = false;
                            newYearStatistic.IsYear = true;
                            newYearStatistic.Type = TypeConstants.ORDER;
                            newYearStatistic.Month = month;
                            newYearStatistic.Value = value;

                            yearOrderList.Add(newYearStatistic);
                        }

                        await _platformStatisticRepository.CreatePlatformStatisticAsync(yearOrderList);

                        break;

                    case UpdateStatisticOption.PROFIT:

                        var monthProfitIds = await _platformStatisticRepository.GetPlatformProfitStatisticIdsByMonthAsync();

                        var yearProfitIds = await _platformStatisticRepository.GetPlatformProfitStatisticIdsByYearAsync();

                        if (monthProfitIds!.Length != 0)
                        {
                            await _platformStatisticRepository.DeletePlatformStatisticAsync(monthProfitIds.ToList());
                        }

                        if (yearProfitIds!.Length != 0)
                        {
                            await _platformStatisticRepository.DeletePlatformStatisticAsync(yearProfitIds.ToList());
                        }

                        var monthProfitStatistic = await _transactionRepository.SumMonthProfitAsync();
                        var yearProfitStatistic = await _transactionRepository.SumYearProfitAsync();

                        List<PlatformStatistic> monthProfitList = new List<PlatformStatistic>();
                        List<PlatformStatistic> yearProfitList = new List<PlatformStatistic>();

                        foreach (var item in monthProfitStatistic)
                        {
                            var day = item.Key;
                            var value = item.Value;

                            var newMonthStatistic = new PlatformStatistic();
                            newMonthStatistic.Month = DateTime.Now.Month;
                            newMonthStatistic.Year = DateTime.Now.Year;
                            newMonthStatistic.IsMonth = true;
                            newMonthStatistic.IsYear = false;
                            newMonthStatistic.Type = TypeConstants.PROFIT;
                            newMonthStatistic.Date = day;
                            newMonthStatistic.Value = value;

                            monthProfitList.Add(newMonthStatistic);
                        }

                        await _platformStatisticRepository.CreatePlatformStatisticAsync(monthProfitList);

                        foreach (var item in yearProfitStatistic)
                        {
                            var month = item.Key;
                            var value = item.Value;

                            var newYearStatistic = new PlatformStatistic();
                            newYearStatistic.Date = 0;
                            newYearStatistic.Year = DateTime.Now.Year;
                            newYearStatistic.IsMonth = false;
                            newYearStatistic.IsYear = true;
                            newYearStatistic.Type = TypeConstants.PROFIT;
                            newYearStatistic.Month = month;
                            newYearStatistic.Value = value;

                            yearProfitList.Add(newYearStatistic);
                        }

                        await _platformStatisticRepository.CreatePlatformStatisticAsync(yearProfitList);

                        break;

                    case UpdateStatisticOption.USER:

                        var isUserEmpty = await _platformStatisticRepository.CheckPlatformUserStatisticAsync();

                        if (isUserEmpty == false)
                        {
                            var currentAccountCount = await _accountRepository.CountAccountDataAsync();

                            var userStatistics = Enumerable.Range(1, 12)
                                .Select(month => new PlatformStatistic
                                {
                                    Value = month == currentMonth ? currentAccountCount : 0,
                                    Month = month,
                                    Year = currentYear,
                                    IsMonth = false,
                                    IsYear = true,
                                    Type = TypeConstants.USER
                                }).ToList();

                            await _platformStatisticRepository.CreatePlatformStatisticAsync(userStatistics);
                        }
                        else
                        {
                            var statistic = await _platformStatisticRepository.GetPlatformUserStatisticByMonthAsync();

                            if (statistic != null)
                            {
                                statistic.Value = await _accountRepository.CountAccountDataAsync();

                                await _platformStatisticRepository.UpdatePlatformStatisticAsync(statistic);
                            }
                        }

                        break;
                }

                return new ApiResponse<PlatformStatisticResponse>("success", "Cập Nhập Thông Tin Thống Kê Thành Công!", null);
            }
            catch (Exception)
            {
                return new ApiResponse<PlatformStatisticResponse>("error", 400, "Cập Nhập Thông Tin Thống Kê Thất Bại!");
            }
        }
    }
}
