using Microsoft.AspNetCore.Mvc;
using TP4SCS.Library.Models.Request.Notification;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [Route("api/order-notifications")]
    [ApiController]
    public class OrderNotificationController : ControllerBase
    {
        private readonly IOrderNotificationService _orderNotificationService;

        public OrderNotificationController(IOrderNotificationService orderNotificationService)
        {
            _orderNotificationService = orderNotificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderNotifcationsAsync([FromQuery] GetOrderNotificationRequest getOrderNotificationRequest)
        {
            var result = await _orderNotificationService.GetOrderNotificationsAsync(getOrderNotificationRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteOrderNotificationsAsync([FromQuery] int[] id)
        {
            var result = await _orderNotificationService.DeleteOrderNotificationsAsync(id);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }
    }
}
