using Mapster;
using MapsterMapper;
using TP4SCS.Library.Models.Request.Notification;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.Notification;
using TP4SCS.Repository.Interfaces;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class OrderNotificationService : IOrderNotificationService
    {
        private readonly IOrderNotificationRepository _orderNotificationRepository;
        private readonly IMapper _mapper;

        public OrderNotificationService(IOrderNotificationRepository orderNotificationRepository, IMapper mapper)
        {
            _orderNotificationRepository = orderNotificationRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<OrderNotificationResponse>> DeleteOrderNotificationsAsync(int[] ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    var notification = await _orderNotificationRepository.GetOrderNotificationByIdAsync(id);

                    if (notification == null)
                    {
                        return new ApiResponse<OrderNotificationResponse>("error", 404, "Không Tìm Thấy Thông Báo Đơn Hàng!");
                    }

                    await _orderNotificationRepository.DeleteOrderNotificationAsync(id);
                }

                return new ApiResponse<OrderNotificationResponse>("success", "Xoá Thông Báo Đơn Hàng Thành Công!", null);
            }
            catch (Exception)
            {
                return new ApiResponse<OrderNotificationResponse>("error", 400, "Xoá Thông Báo Đơn Hàng Thất Bại!");
            }
        }

        public async Task<ApiResponse<IEnumerable<OrderNotificationResponse>?>> GetOrderNotificationsAsync(GetOrderNotificationRequest getOrderNotificationRequest)
        {
            var (notifications, pagination) = await _orderNotificationRepository.GetOrderNotificationsAsync(getOrderNotificationRequest);

            if (notifications == null)
            {
                return new ApiResponse<IEnumerable<OrderNotificationResponse>?>("error", 404, "Thông Báo Đơn Hàng Trống!");
            }

            var data = notifications.Adapt<IEnumerable<OrderNotificationResponse>>();

            return new ApiResponse<IEnumerable<OrderNotificationResponse>?>("success", "Lấy Thông Báo Đơn Hàng Thành Công!", data, 200, pagination);
        }
    }
}
