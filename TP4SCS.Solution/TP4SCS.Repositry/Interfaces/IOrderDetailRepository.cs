using TP4SCS.Library.Models.Data;

namespace TP4SCS.Repository.Interfaces
{
    public interface IOrderDetailRepository
    {
        Task AddOrderDetailsAsync(List<OrderDetail> orderDetails);
        Task AddOrderDetailAsync(OrderDetail orderDetail);
        Task<OrderDetail?> GetOrderDetailByIdAsync(int id);
        Task<IEnumerable<OrderDetail>> GetOrderDetailsByOrderIdAsync(int orderId);
        Task UpdateOrderDetailAsync(OrderDetail orderDetail);
        Task DeleteOrderDetailAsync(int id);
    }
}
