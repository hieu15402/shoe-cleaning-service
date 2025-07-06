using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.OrderDetail;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.Material;
using TP4SCS.Library.Models.Response.OrderDetail;
using TP4SCS.Library.Utils.Utils;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [ApiController]
    public class OrderDetailController : ControllerBase
    {
        private readonly IOrderDetailService _orderDetailService;
        private readonly IMaterialService _materialService;
        private readonly IMapper _mapper;
        public OrderDetailController(IOrderDetailService orderDetailService, IMapper mapper, IMaterialService materialService)
        {
            _orderDetailService = orderDetailService;
            _materialService = materialService;
            _mapper = mapper;
        }

        [HttpGet]
        [Route("api/orderdetails/{id}")]
        public async Task<IActionResult> GetOrderDetailByIdAsync(int id)
        {
            try
            {
                var orderDetail = await _orderDetailService.GetOrderDetailByIdAsync(id);
                if (orderDetail == null)
                {
                    return NotFound(new ResponseObject<OrderDetailResponseV2>($"Không tìm thấy chi tiết đơn hàng với ID {id}.", null));
                }

                var response = _mapper.Map<OrderDetailResponseV2>(orderDetail);
                if (!string.IsNullOrEmpty(orderDetail.MaterialIds))
                {
                    List<int> materialIds = Util.ConvertStringToList(orderDetail.MaterialIds);
                    var materials = await _materialService.GetMaterialsByIdsAsync(materialIds);
                    List<MaterialResponseV2> materialResponse = _mapper.Map<IEnumerable<MaterialResponseV2>>(materials).ToList();
                    response.Materials = materialResponse;
                }

                return Ok(new ResponseObject<OrderDetailResponseV2>("Lấy chi tiết đơn hàng thành công.", response));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Đã xảy ra lỗi: {ex.Message}"));
            }
        }

        [HttpGet]
        [Route("api/order/{id}/orderdetails")]
        public async Task<IActionResult> GetOrderDetailsByOrderIdAsync(int id)
        {
            try
            {
                var orderDetails = await _orderDetailService.GetOrderDetailsByOrderIdAsync(id);
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

                return Ok(new ResponseObject<IEnumerable<OrderDetailResponseV2>>("Lấy danh sách chi tiết đơn hàng thành công.", responseList));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Đã xảy ra lỗi: {ex.Message}"));
            }
        }

        [HttpPost]
        [Route("api/orderdetails")]
        public async Task<IActionResult> AddOrderDetailsAsync([FromBody] OrderDetailCreateRequest request)
        {
            try
            {
                await _orderDetailService.AddOrderDetailAsync(request.OrderId, request.BranchId, request.ServiceId, request.MaterialIds);
                return Ok(new ResponseObject<string>("Thêm chi tiết đơn hàng thành công."));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ResponseObject<string>($"Lỗi hợp lệ: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Đã xảy ra lỗi: {ex.Message}"));
            }
        }
        [HttpPatch("api/orderdetails/{id}")]
        public async Task<IActionResult> UpdateOrderDetail(int id, [FromBody] OrderDetailUpdateRequest request)
        {
            try
            {
                var od = _mapper.Map<OrderDetail>(request);
                await _orderDetailService.UpdateOrderDetailAsync(id, od);
                return Ok(new ResponseObject<string>("Cập nhật thành công"));
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new ResponseObject<string>(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>(ex.Message));
            }
        }
        [HttpDelete]
        [Route("api/orderdetails/{id}")]
        public async Task<IActionResult> DeleteOrderDetailAsync(int id)
        {
            try
            {
                await _orderDetailService.DeleteOrderDetailAsync(id);
                return Ok(new ResponseObject<string>("Xóa chi tiết đơn hàng thành công."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseObject<string>(ex.Message));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new ResponseObject<string>($"Lỗi hợp lệ: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return NotFound(new ResponseObject<string>($"Lỗi: {ex.Message}"));
            }
        }
    }
}
