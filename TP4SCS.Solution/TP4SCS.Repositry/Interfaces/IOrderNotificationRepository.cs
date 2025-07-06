using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Notification;
using TP4SCS.Library.Models.Response.General;

namespace TP4SCS.Repository.Interfaces
{
    public interface IOrderNotificationRepository : IGenericRepository<OrderNotification>
    {
        Task<(IEnumerable<OrderNotification>?, Pagination)> GetOrderNotificationsAsync(GetOrderNotificationRequest getOrderNotificationRequest);

        Task<OrderNotification?> GetOrderNotificationByIdAsync(int id);

        Task CreateOrderNotificationAsync(OrderNotification orderNotification);

        Task DeleteOrderNotificationAsync(int id);
    }
}
