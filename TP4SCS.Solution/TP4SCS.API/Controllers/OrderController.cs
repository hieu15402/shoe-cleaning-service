using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Library.Models.Request.Order;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.Material;
using TP4SCS.Library.Models.Response.Order;
using TP4SCS.Library.Models.Response.OrderDetail;
using TP4SCS.Library.Utils.Utils;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [Route("api/orders")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IEmailService _emailService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IMaterialService _materialService;
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;

        public OrderController(IOrderService orderService,
            IEmailService emailService,
            IMapper mapper,
            IHttpClientFactory httpClientFactory,
            IOrderDetailService orderDetailService,
            IMaterialService materialService)
        {
            _orderService = orderService;
            _emailService = emailService;
            _httpClient = httpClientFactory.CreateClient();
            _mapper = mapper;
            _orderDetailService = orderDetailService;
            _materialService = materialService;
        }

        [HttpGet]
        public async Task<IActionResult> GetOrdersAsync(
            [FromQuery] string? status = null,
            [FromQuery] int? pageIndex = null,
            [FromQuery] int? pageSize = null,
            [FromQuery] OrderedOrderByEnum orderBy = OrderedOrderByEnum.CreateDateAsc)
        {
            try
            {
                // Lấy danh sách orders từ service
                var orders = await _orderService.GetOrdersAsync(status, pageIndex, pageSize, orderBy);
                var orderResponses = new List<OrderResponse>();
                if (orders == null || !orders.Any())
                {
                    return Ok(new ResponseObject<IEnumerable<OrderResponse>>("Không tìm thấy danh sách đơn đặt hàng", orderResponses));
                }
                foreach (var order in orders)
                {
                    // Ánh xạ thông tin cơ bản của Order
                    var orderResponse = _mapper.Map<OrderResponse>(order);

                    var orderDetails = await _orderDetailService.GetOrderDetailsByOrderIdAsync(order.Id);
                    if (orderDetails == null || !orderDetails.Any())
                    {
                        return NotFound(new ResponseObject<IEnumerable<OrderDetailResponseV2>>("Không tìm thấy chi tiết đơn hàng cho đơn hàng này."));
                    }

                    var responseList = new List<OrderDetailResponseV2>();

                    foreach (var orderDetail in orderDetails)
                    {
                        var response = _mapper.Map<OrderDetailResponseV2>(orderDetail);

                        if (!string.IsNullOrEmpty(orderDetail.MaterialIds))
                        {
                            List<int> materialIds = Util.ConvertStringToList(orderDetail.MaterialIds);
                            var materials = await _materialService.GetMaterialsByIdsAsync(materialIds);
                            List<MaterialResponseV2> materialResponse = _mapper.Map<IEnumerable<MaterialResponseV2>>(materials).ToList();
                            response.Materials = materialResponse;
                        }

                        responseList.Add(response);
                    }
                    orderResponse.OrderDetails = responseList;
                    orderResponses.Add(orderResponse);
                }

                return Ok(new ResponseObject<IEnumerable<OrderResponse>>("Lấy danh sách đơn hàng thành công.", orderResponses));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Đã xảy ra lỗi: {ex.Message}"));
            }
        }

        [HttpGet("accounts/{accountId}")]
        public async Task<IActionResult> GetOrdersByAccountIdAsync(
            int accountId,
            [FromQuery] string? status = null,
            [FromQuery] OrderedOrderByEnum orderBy = OrderedOrderByEnum.CreateDateAsc)
        {
            try
            {
                var orders = await _orderService.GetOrdersByAccountIdAsync(accountId, status, orderBy);
                var orderResponses = new List<OrderResponse>();
                if (orders == null || !orders.Any())
                {
                    return Ok(new ResponseObject<IEnumerable<OrderResponse>>("Không tìm thấy danh sách đơn đặt hàng", orderResponses));
                }
                foreach (var order in orders)
                {
                    // Ánh xạ thông tin cơ bản của Order
                    var orderResponse = _mapper.Map<OrderResponse>(order);

                    var orderDetails = await _orderDetailService.GetOrderDetailsByOrderIdAsync(order.Id);
                    if (orderDetails == null || !orderDetails.Any())
                    {
                        return NotFound(new ResponseObject<IEnumerable<OrderDetailResponseV2>>("Không tìm thấy chi tiết đơn hàng cho đơn hàng này."));
                    }

                    var responseList = new List<OrderDetailResponseV2>();

                    foreach (var orderDetail in orderDetails)
                    {
                        var response = _mapper.Map<OrderDetailResponseV2>(orderDetail);

                        if (!string.IsNullOrEmpty(orderDetail.MaterialIds))
                        {
                            List<int> materialIds = Util.ConvertStringToList(orderDetail.MaterialIds);
                            var materials = await _materialService.GetMaterialsByIdsAsync(materialIds);
                            List<MaterialResponseV2> materialResponse = _mapper.Map<IEnumerable<MaterialResponseV2>>(materials).ToList();
                            response.Materials = materialResponse;
                        }

                        responseList.Add(response);
                    }
                    orderResponse.OrderDetails = responseList;
                    orderResponses.Add(orderResponse);
                }
                return Ok(new ResponseObject<IEnumerable<OrderResponse>>("Lấy đơn hàng theo tài khoản thành công.", orderResponses));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Đã xảy ra lỗi: {ex.Message}"));
            }
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderByIdAsync(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByOrderId(id);
                if (order == null)
                {
                    return Ok(new ResponseObject<IEnumerable<OrderResponse>>("Không tìm thấy danh sách đơn đặt hàng", new List<OrderResponse>()));
                }
                var orderResponse = _mapper.Map<OrderResponse>(order);

                var orderDetails = await _orderDetailService.GetOrderDetailsByOrderIdAsync(order.Id);
                if (orderDetails == null || !orderDetails.Any())
                {
                    return Ok(new ResponseObject<IEnumerable<OrderResponse>>("Danh sách đơn đặt hàng rỗng", new List<OrderResponse>()));
                }

                var responseList = new List<OrderDetailResponseV2>();

                foreach (var orderDetail in orderDetails)
                {
                    var response = _mapper.Map<OrderDetailResponseV2>(orderDetail);

                    if (!string.IsNullOrEmpty(orderDetail.MaterialIds))
                    {
                        List<int> materialIds = Util.ConvertStringToList(orderDetail.MaterialIds);
                        var materials = await _materialService.GetMaterialsByIdsAsync(materialIds);
                        List<MaterialResponseV2> materialResponse = _mapper.Map<IEnumerable<MaterialResponseV2>>(materials).ToList();
                        response.Materials = materialResponse;
                    }

                    responseList.Add(response);
                }
                orderResponse.OrderDetails = responseList;
                return Ok(new ResponseObject<OrderResponse>("Lấy danh sách đơn hàng theo id thành công.", orderResponse));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Đã xảy ra lỗi: {ex.Message}"));
            }
        }
        [HttpGet("ship-code/{code}")]
        public async Task<IActionResult> GetOrderByShipCodeAsync([FromRoute]string code)
        {
            try
            {
                var order = await _orderService.GetOrderByShipCode(code);
                if (order == null)
                {
                    return Ok(new ResponseObject<IEnumerable<OrderResponse>>("Không tìm thấy danh sách đơn đặt hàng", new List<OrderResponse>()));
                }
                var orderResponse = _mapper.Map<OrderResponse>(order);

                var orderDetails = await _orderDetailService.GetOrderDetailsByOrderIdAsync(order.Id);
                if (orderDetails == null || !orderDetails.Any())
                {
                    return Ok(new ResponseObject<IEnumerable<OrderResponse>>("Danh sách đơn đặt hàng rỗng", new List<OrderResponse>()));
                }

                var responseList = new List<OrderDetailResponseV2>();

                foreach (var orderDetail in orderDetails)
                {
                    var response = _mapper.Map<OrderDetailResponseV2>(orderDetail);

                    if (!string.IsNullOrEmpty(orderDetail.MaterialIds))
                    {
                        List<int> materialIds = Util.ConvertStringToList(orderDetail.MaterialIds);
                        var materials = await _materialService.GetMaterialsByIdsAsync(materialIds);
                        List<MaterialResponseV2> materialResponse = _mapper.Map<IEnumerable<MaterialResponseV2>>(materials).ToList();
                        response.Materials = materialResponse;
                    }

                    responseList.Add(response);
                }
                orderResponse.OrderDetails = responseList;
                return Ok(new ResponseObject<OrderResponse>("Lấy danh sách đơn hàng theo id thành công.", orderResponse));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Đã xảy ra lỗi: {ex.Message}"));
            }
        }
        [HttpGet("branches/{id}")]
        public async Task<IActionResult> GetOrdersByBranchIdAsync(
            int id,
            [FromQuery] string? status = null,
            [FromQuery] OrderedOrderByEnum orderBy = OrderedOrderByEnum.CreateDateAsc)
        {
            try
            {
                var orders = await _orderService.GetOrdersByBranchIdAsync(id, status, orderBy);
                var orderResponses = new List<OrderResponse>();
                if (orders == null || !orders.Any())
                {
                    return Ok(new ResponseObject<IEnumerable<OrderResponse>>("Không tìm thấy danh sách đơn đặt hàng", orderResponses));
                }
                foreach (var order in orders)
                {
                    // Ánh xạ thông tin cơ bản của Order
                    var orderResponse = _mapper.Map<OrderResponse>(order);

                    var orderDetails = await _orderDetailService.GetOrderDetailsByOrderIdAsync(order.Id);
                    if (orderDetails == null || !orderDetails.Any())
                    {
                        return Ok(new ResponseObject<IEnumerable<OrderResponse>>("Danh sách đơn đặt hàng rỗng", orderResponses));
                    }

                    var responseList = new List<OrderDetailResponseV2>();

                    foreach (var orderDetail in orderDetails)
                    {
                        var response = _mapper.Map<OrderDetailResponseV2>(orderDetail);

                        if (!string.IsNullOrEmpty(orderDetail.MaterialIds))
                        {
                            List<int> materialIds = Util.ConvertStringToList(orderDetail.MaterialIds);
                            var materials = await _materialService.GetMaterialsByIdsAsync(materialIds);
                            List<MaterialResponseV2> materialResponse = _mapper.Map<IEnumerable<MaterialResponseV2>>(materials).ToList();
                            response.Materials = materialResponse;
                        }

                        responseList.Add(response);
                    }
                    orderResponse.OrderDetails = responseList;
                    orderResponses.Add(orderResponse);
                }
                return Ok(new ResponseObject<IEnumerable<OrderResponse>>("Lấy danh sách đơn hàng theo chi nhánh thành công.", orderResponses));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Đã xảy ra lỗi: {ex.Message}"));
            }
        }

        [HttpGet("businesses/{id}")]
        public async Task<IActionResult> GetOrdersByBusinessIdAsync(
            int id,
            [FromQuery] string? status = null,
            [FromQuery] OrderedOrderByEnum orderBy = OrderedOrderByEnum.CreateDateAsc)
        {
            try
            {
                var orders = await _orderService.GetOrdersByBusinessIdAsync(id, status, orderBy);
                var orderResponses = new List<OrderResponse>();
                if (orders == null || !orders.Any())
                {
                    return Ok(new ResponseObject<IEnumerable<OrderResponse>>("Không tìm thấy danh sách đơn đặt hàng", orderResponses));
                }
                foreach (var order in orders)
                {
                    // Ánh xạ thông tin cơ bản của Order
                    var orderResponse = _mapper.Map<OrderResponse>(order);

                    var orderDetails = await _orderDetailService.GetOrderDetailsByOrderIdAsync(order.Id);
                    if (orderDetails == null || !orderDetails.Any())
                    {
                        return Ok(new ResponseObject<IEnumerable<OrderResponse>>("Danh sách đơn đặt hàng rỗng", orderResponses));
                    }

                    var responseList = new List<OrderDetailResponseV2>();

                    foreach (var orderDetail in orderDetails)
                    {
                        var response = _mapper.Map<OrderDetailResponseV2>(orderDetail);

                        if (!string.IsNullOrEmpty(orderDetail.MaterialIds))
                        {
                            List<int> materialIds = Util.ConvertStringToList(orderDetail.MaterialIds);
                            var materials = await _materialService.GetMaterialsByIdsAsync(materialIds);
                            List<MaterialResponseV2> materialResponse = _mapper.Map<IEnumerable<MaterialResponseV2>>(materials).ToList();
                            response.Materials = materialResponse;
                        }

                        responseList.Add(response);
                    }
                    orderResponse.OrderDetails = responseList;
                    orderResponses.Add(orderResponse);
                }
                return Ok(new ResponseObject<IEnumerable<OrderResponse>>("Lấy danh sách đơn hàng theo doanh nghiệp thành công.", orderResponses));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Đã xảy ra lỗi: {ex.Message}"));
            }
        }

        [HttpPost("{id}/send-email-approved")]
        public async Task<IActionResult> Approved(int id, string toEmail)
        {
            try
            {
                // Thiết lập subject và body cho email
                string subject = "Đơn hàng đã được duyệt!";
                string body = $"Chào bạn,\n\n" +
                              $"Đơn hàng #{id} của bạn đã được duyệt thành công.\n" +
                              "Cảm ơn bạn đã chọn dịch vụ của chúng tôi.\n\n" +
                              "Trân trọng,\n" +
                              "Đội ngũ hỗ trợ khách hàng";

                // Gửi email
                await _emailService.SendEmailAsync(toEmail, subject, body);

                // Trả về thông báo thành công dưới dạng ResponseObject
                return Ok(new ResponseObject<string>("Gửi email thành công!"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Lỗi khi gửi email: {ex.Message}"));
            }
        }
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            try
            {
                // Cập nhật trạng thái đơn hàng
                await _orderService.UpdateOrderStatusAsync(_httpClient, id, status);

                // Trả về thông báo thành công dưới dạng ResponseObject
                return Ok(new ResponseObject<string>("Cập nhật trạng thái đơn hàng thành công!", null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Lỗi khi cập nhật trạng thái đơn hàng: {ex.Message}"));
            }
        }
        [HttpPut("{id}/ship-code")]
        public async Task<IActionResult> UpdateShipCode(int id)
        {
            try
            {
                await _orderService.CreateShipOrder(_httpClient, id);

                return Ok(new ResponseObject<string>("Cập nhật mã vận đơn của đơn hàng thành công!", null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Lỗi khi cập nhật mã vận đơn của đơn hàng: {ex.Message}"));
            }
        }
        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateOrderAsync(int id, UpdateOrderRequest request)
        {
            try
            {
                await _orderService.UpdateOrderAsync(id, request);
                return Ok(new ResponseObject<string>("Đơn hàng đã được cập nhật thành công!", null));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseObject<string>(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Lỗi khi cập nhật: {ex.Message}", null));
            }
        }
    }
}
