using Mapster;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Notification;
using TP4SCS.Library.Models.Response.Notification;

namespace TP4SCS.Library.Utils.Mapper
{
    internal class OrderNotificationMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<OrderNotification, OrderNotificationResponse>();

            config.NewConfig<CreateOrderNotificationRequest, OrderNotification>()
                .Map(dest => dest.OrderId, src => src.OrderId)
                .Map(dest => dest.NotificationTime, otp => DateTime.Now)
                .Map(dest => dest.Title, otp => "Thông Báo Đơn Hàng");
        }
    }
}
