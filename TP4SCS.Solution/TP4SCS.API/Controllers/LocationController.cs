using Microsoft.AspNetCore.Mvc;
using TP4SCS.Library.Models.Request.ShipFee;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [ApiController]
    public class LocationController : ControllerBase
    {
        private readonly IShipService _shipService;
        private readonly HttpClient _httpClient;

        public LocationController(IShipService shipService, IHttpClientFactory httpClientFactory)
        {
            _shipService = shipService;
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpGet]
        [Route("api/locations/provinces")]
        public async Task<IActionResult> GetProvincesAsync()
        {
            var result = await _shipService.GetProvincesAsync(_httpClient);

            if (result == null)
            {
                return NotFound(new
                {
                    status = "error",
                    statusCode = 404,
                    message = "Không Tìm Thấy Dữ Liệu Tỉnh!"
                });
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("api/locations/{provinceId}/districts")]
        public async Task<IActionResult> GetDistrictsByProvinceIdAsync([FromRoute] int provinceId)
        {
            var result = await _shipService.GetDistrictsByProvinceIdAsync(_httpClient, provinceId);

            if (result == null)
            {
                return NotFound(new
                {
                    status = "error",
                    statusCode = 404,
                    message = "Không Tìm Thấy Dữ Liệu Quận!"
                });
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("api/locations/{districtId}/wards")]
        public async Task<IActionResult> GetWardsByDistrictIdAsync([FromRoute] int districtId)
        {
            var result = await _shipService.GetWardsByDistrictIdAsync(_httpClient, districtId);

            if (result == null)
            {
                return NotFound(new
                {
                    status = "error",
                    statusCode = 404,
                    message = "Không Tìm Thấy Dữ Liệu Phường!"
                });
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("api/order-ship/status")]
        public async Task<IActionResult> GetOrderStatusAsync([FromQuery] string orderCode)
        {
            // Gọi hàm GetOrderStatusAsync từ service
            var (status, logs) = await _shipService.GetOrderStatusAsync(_httpClient, orderCode);

            // Nếu không tìm thấy kết quả
            if (status == null)
            {
                return NotFound(new
                {
                    status = "error",
                    statusCode = 404,
                    message = "Không tìm thấy thông tin trạng thái đơn hàng!"
                });
            }

            // Trả về kết quả thành công
            return Ok(new
            {
                status = "success",
                statusCode = 200,
                message = "Lấy trạng thái đơn hàng thành công!",
                data = new
                {
                    Status = status,
                    Logs = logs.Select(log => new // Chuyển logs thành các đối tượng rõ ràng
                    {
                        Status = log.Status,
                        UpdatedDate = log.UpdatedDate
                    }).ToList() // Đảm bảo logs là một danh sách các đối tượng JSON hợp lệ
                }
            });
        }

        [HttpPost]
        [Route("api/order-ship")]
        public async Task<IActionResult> CreateShippingOrderAsync([FromBody] ShippingOrderRequest request)
        {
            var result = await _shipService.CreateShippingOrderAsync(_httpClient, request);

            if (result == null)
            {
                return NotFound(new
                {
                    status = "error",
                    statusCode = 404,
                    message = "Không Tìm Thấy Dữ Liệu Phường!"
                });
            }

            return Ok(new ResponseObject<string>(result));
        }
    }
}
