using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Library.Models.Request.Order;

namespace TP4SCS.Services.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>?> GetOrdersByBranchIdAsync(
            int branchId,
            string? status = null,
            OrderedOrderByEnum orderBy = OrderedOrderByEnum.CreateDateAsc);
        Task<IEnumerable<Order>?> GetOrdersByAccountIdAsync(
            int accountId,
            string? status = null,
            OrderedOrderByEnum orderBy = OrderedOrderByEnum.CreateDateAsc);
        Task<IEnumerable<Order>?> GetOrdersAsync(
            string? status = null,
            int? pageIndex = null,
            int? pageSize = null,
            OrderedOrderByEnum orderBy = OrderedOrderByEnum.CreateDateAsc);
        Task<IEnumerable<Order>?> GetOrdersByBusinessIdAsync(
            int businessId,
            string? status = null,
            OrderedOrderByEnum orderBy = OrderedOrderByEnum.CreateDateAsc);
        Task UpdateOrderStatusAsync(HttpClient httpClient, int existingOrderedId, string newStatus);
        //        Task ApprovedOrder(int orderId);
        Task UpdateOrderAsync(int existingOrderId, UpdateOrderRequest request);
        Task<Order?> GetOrderByOrderId(int orderId);
        Task CreateShipOrder(HttpClient httpClient, int orderId);
        Task<Order?> GetOrderByShipCode(string shipCode);
    }
}
