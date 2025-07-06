using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Repository.Interfaces;

namespace TP4SCS.Repository.Implements
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        public OrderRepository(Tp4scsDevDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task AddOrdersAsync(List<Order> orders)
        {
            await _dbContext.AddRangeAsync(orders);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(int id)
        {
            await DeleteAsync(id);
        }

        public async Task<(int, int)> GetBranchIdAndBusinessIdByOrderId(int id)
        {
            var result = await _dbContext.OrderDetails
                .AsNoTracking()
                .Include(od => od.Branch)
                .Where(od => od.OrderId == id)
                .FirstOrDefaultAsync();

            if (result == null || result.Branch == null)
            {
                return (0, 0); // Trả về (0, 0) nếu không tìm thấy đơn hàng hoặc không có chi nhánh
            }

            // Trả về BranchId và BusinessId từ Branch
            return (result.Branch.Id, result.Branch.BusinessId);
        }
        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _dbContext.Orders
                    .Include(o => o.Account)
                        .ThenInclude(a => a.AccountAddresses)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Service)
                            .ThenInclude(s => s!.Promotion)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Service)
                            .ThenInclude(s => s!.Category)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Service)
                            .ThenInclude(s => s!.AssetUrls)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Service)
                            .ThenInclude(s => s!.BranchServices)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Branch)
                    .Include(o => o.OrderDetails)
                    //.ThenInclude(od => od.Material)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Feedback)
                .SingleOrDefaultAsync(o => o.Id == id);
        }
        public async Task<Order?> GetOrderByCodeAsync(string code)
        {
            var order = await _dbContext.Orders
                .Where(o => o.ShippingCode != null && o.ShippingCode.Contains(code))
                .SingleOrDefaultAsync();
            return order;
        }
        public async Task<IEnumerable<Order>?> GetOrdersAsync(
            string? status = null,
            int? pageIndex = null,
            int? pageSize = null,
            OrderedOrderByEnum orderBy = OrderedOrderByEnum.CreateDateAsc)
        {
            // Filter by status and accountId
            Expression<Func<Order, bool>> filter = o =>
                (string.IsNullOrEmpty(status) || o.Status.ToLower().Trim() == status.ToLower().Trim());

            // Sort based on OrderByEnum
            Func<IQueryable<Order>, IOrderedQueryable<Order>> orderByExpression = q => orderBy switch
            {
                OrderedOrderByEnum.IdDesc => q.OrderByDescending(o => o.Id),
                OrderedOrderByEnum.IdAsc => q.OrderBy(o => o.Id),
                OrderedOrderByEnum.CreateDateDes => q.OrderByDescending(o => o.CreateTime),
                _ => q.OrderBy(o => o.CreateTime)
            };

            var query = _dbSet.OrderByDescending(o => o.CreateTime).Where(filter);

            // Bao gồm các thuộc tính liên quan
            query = query
                    .Include(o => o.Account)
                        .ThenInclude(a => a.AccountAddresses)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Service)
                            .ThenInclude(s => s!.Promotion)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Service)
                            .ThenInclude(s => s!.Category)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Service)
                            .ThenInclude(s => s!.AssetUrls)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Service)
                            .ThenInclude(s => s!.BranchServices)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Branch)
                    .Include(o => o.OrderDetails)
                    //.ThenInclude(od => od.Material)
                    .Include(o => o.OrderDetails)
                        .ThenInclude(od => od.Feedback)
                            .ThenInclude(f => f!.AssetUrls);

            // Thực hiện phân trang nếu có pageIndex và pageSize
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10;

                query = query.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            return await query.ToListAsync();
        }

        public async Task UpdateOrderAsync(Order order)
        {
            await UpdateAsync(order);
        }

        public async Task<int> CountMonthOrderByBusinessIdAsync(int id)
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            return await _dbContext.Orders
                .AsNoTracking()
                .Where(o => o.OrderDetails
                    .Any(od => od.Branch.Business.Id == id) &&
                    o.FinishedTime.HasValue &&
                    o.FinishedTime.Value.Month == currentMonth &&
                    o.FinishedTime.Value.Year == currentYear)
                .CountAsync();
        }

        public async Task<int> CountYearOrderByBusinessIdAsync(int id)
        {
            var currentYear = DateTime.Now.Year;

            return await _dbContext.Orders
                .AsNoTracking()
                .Where(o => o.OrderDetails
                    .Any(od => od.Branch.Business.Id == id) &&
                    o.FinishedTime.HasValue &&
                    o.FinishedTime.Value.Year == currentYear)
                .CountAsync();
        }

        public async Task<Dictionary<int, int>> CountMonthOrdersByBusinessIdAsync(int id)
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var orders = await _dbContext.Orders
                .AsNoTracking()
                .Where(o =>
                    o.OrderDetails.Any(od => od.Branch.Business.Id == id) &&
                    o.FinishedTime.HasValue &&
                    o.FinishedTime.Value.Month == currentMonth &&
                    o.FinishedTime.Value.Year == currentYear)
                .ToListAsync();

            var dailyCounts = orders
                .GroupBy(o => o.FinishedTime!.Value.Day)
                .ToDictionary(
                    group => group.Key,
                    group => group.Count()
                );

            var result = Enumerable.Range(1, 31)
                .ToDictionary(
                    day => day,
                    day => dailyCounts.ContainsKey(day) ? dailyCounts[day] : 0
                );

            return result;
        }

        public async Task<Dictionary<int, int>> CountMonthOrdersAsync()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var orders = await _dbContext.Orders
                .AsNoTracking()
                .Where(o =>
                    o.FinishedTime.HasValue &&
                    o.FinishedTime.Value.Month == currentMonth &&
                    o.FinishedTime.Value.Year == currentYear)
                .ToListAsync();

            var dailyCounts = orders
                .GroupBy(o => o.FinishedTime!.Value.Day)
                .ToDictionary(
                    group => group.Key,
                    group => group.Count()
                );

            var result = Enumerable.Range(1, 31)
                .ToDictionary(
                    day => day,
                    day => dailyCounts.ContainsKey(day) ? dailyCounts[day] : 0
                );

            return result;
        }

        public async Task<Dictionary<int, int>> CountYearOrdersByBusinessIdAsync(int id)
        {
            var currentYear = DateTime.Now.Year;

            var orders = await _dbContext.Orders
                .AsNoTracking()
                .Where(o =>
                    o.OrderDetails.Any(od => od.Branch.Business.Id == id) &&
                    o.FinishedTime.HasValue &&
                    o.FinishedTime.Value.Year == currentYear)
                .ToListAsync();

            var monthlyCounts = orders
                .GroupBy(o => o.FinishedTime!.Value.Month)
                .ToDictionary(
                    group => group.Key,
                    group => group.Count()
                );

            var result = Enumerable.Range(1, 12)
                .ToDictionary(
                    month => month,
                    month => monthlyCounts.ContainsKey(month) ? monthlyCounts[month] : 0
                );

            return result;
        }

        public async Task<Dictionary<int, int>> CountYearOrdersAsync()
        {
            var currentYear = DateTime.Now.Year;

            var orders = await _dbContext.Orders
                .AsNoTracking()
                .Where(o =>
                    o.FinishedTime.HasValue &&
                    o.FinishedTime.Value.Year == currentYear)
                .ToListAsync();

            var monthlyCounts = orders
                .GroupBy(o => o.FinishedTime!.Value.Month)
                .ToDictionary(
                    group => group.Key,
                    group => group.Count()
                );

            var result = Enumerable.Range(1, 12)
                .ToDictionary(
                    month => month,
                    month => monthlyCounts.ContainsKey(month) ? monthlyCounts[month] : 0
                );

            return result;
        }

        public async Task<Dictionary<int, decimal>> SumMonthOrderProfitByBusinessIdAsync(int id)
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var orders = await _dbContext.Orders
                .AsNoTracking()
                .Where(o =>
                    o.OrderDetails.Any(od => od.Branch.Business.Id == id) &&
                    o.FinishedTime.HasValue &&
                    o.FinishedTime.Value.Month == currentMonth &&
                    o.FinishedTime.Value.Year == currentYear)
                .ToListAsync();

            var dailyProfits = orders
                .GroupBy(o => o.FinishedTime!.Value.Day)
                .ToDictionary(
                    group => group.Key,
                    group => group.Sum(o => o.OrderPrice)
                );

            var result = Enumerable.Range(1, 31)
                .ToDictionary(
                    day => day,
                    day => dailyProfits.ContainsKey(day) ? dailyProfits[day] : 0m
                );

            return result;
        }

        public async Task<Dictionary<int, decimal>> SumYearOrderProfitByBusinessIdAsync(int id)
        {
            var currentYear = DateTime.Now.Year;

            var orders = await _dbContext.Orders
                .AsNoTracking()
                .Where(o =>
                    o.OrderDetails.Any(od => od.Branch.Business.Id == id) &&
                    o.FinishedTime.HasValue &&
                    o.FinishedTime.Value.Year == currentYear)
                .ToListAsync();

            var monthlyProfits = orders
                .GroupBy(o => o.FinishedTime!.Value.Month)
                .ToDictionary(
                    group => group.Key,
                    group => group.Sum(o => o.OrderPrice)
                );

            var result = Enumerable.Range(1, 12)
                .ToDictionary(
                    month => month,
                    month => monthlyProfits.ContainsKey(month) ? monthlyProfits[month] : 0m
                );

            return result;
        }

        public async Task<Order?> GetUpdateOrderByIdAsync(int id)
        {
            return await _dbContext.Orders.FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<int> CountOrderByBusinessIdAsync(int id)
        {
            return await _dbContext.Orders
                .AsNoTracking()
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Branch)
                .Where(o => o.OrderDetails.Any(od => od.Branch.BusinessId == id) &&
                    o.Status.Equals(StatusConstants.FINISHED))
                .CountAsync();
        }
    }
}
