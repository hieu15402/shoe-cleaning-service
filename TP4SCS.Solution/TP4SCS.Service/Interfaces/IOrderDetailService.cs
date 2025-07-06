using TP4SCS.Library.Models.Data;

namespace TP4SCS.Services.Interfaces
{
    public interface IOrderDetailService
    {
        //       Task AddOrderDetailsAsync(List<OrderDetail> orderDetails);
        Task AddOrderDetailAsync(int orderId, int branchId, int serviceId, List<int> materialIds);
        Task<OrderDetail?> GetOrderDetailByIdAsync(int id);
        Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId);
        Task DeleteOrderDetailAsync(int id);
        Task UpdateOrderDetailAsync(int existingOrderDetailId, OrderDetail orderDetail);
    }
}
