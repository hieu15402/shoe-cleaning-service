using TP4SCS.Library.Models.Request.Notification;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.Notification;

namespace TP4SCS.Services.Interfaces
{
    public interface IOrderNotificationService
    {
        Task<ApiResponse<IEnumerable<OrderNotificationResponse>?>> GetOrderNotificationsAsync(GetOrderNotificationRequest getOrderNotificationRequest);

        Task<ApiResponse<OrderNotificationResponse>> DeleteOrderNotificationsAsync(int[] ids);
    }
}
