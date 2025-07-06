using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Library.Models.Request.Service;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.Service;
using TP4SCS.Library.Utils.Utils;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [Route("api/services")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IServiceService _serviceService;
        private readonly IBusinessService _businessService;

        public ServiceController(IMapper mapper, IServiceService serviceService, IBusinessService businessService)
        {
            _mapper = mapper;
            _serviceService = serviceService;
            _businessService = businessService;
        }

        [HttpGet]
        public async Task<IActionResult> GetServicesAync([FromQuery] PagedRequest pagedRequest)
        {
            if (pagedRequest.PageIndex == 0 || pagedRequest.PageSize == 0)
            {
                pagedRequest.PageIndex = 1;
                pagedRequest.PageSize = 10;
            }
            var services = await _serviceService.GetServicesIncludeBusinessRankAsync(
                pagedRequest.Keyword,
                pagedRequest.Status,
                pagedRequest.PageIndex,
                pagedRequest.PageSize,
                pagedRequest.OrderBy
            );

            var totalCount = await _serviceService.GetTotalServiceCountAsync(
                pagedRequest.Keyword,
                pagedRequest.Status
            );

            var pagedResponse = new PagedResponse<ServiceResponseV3>(
                services ?? new List<ServiceResponseV3>(),
                totalCount,
                pagedRequest.PageIndex,
                pagedRequest.PageSize
            );

            return Ok(new ResponseObject<PagedResponse<ServiceResponseV3>>("Lấy dịch vụ thành công", pagedResponse));
        }

        [HttpGet]
        [Route("branches/{id}")]
        public async Task<IActionResult> GetServicesByBranchIdAync(int id, [FromQuery] PagedRequest pagedRequest)
        {
            if (pagedRequest.PageIndex == 0 || pagedRequest.PageSize == 0)
            {
                pagedRequest.PageIndex = 1;
                pagedRequest.PageSize = 10;
            }
            // Gọi service và lấy danh sách dịch vụ kèm theo tổng số lượng
            var (services, totalCount) = await _serviceService.GetServicesByBranchIdAsync(
                id,
                pagedRequest.Keyword,
                pagedRequest.Status,
                pagedRequest.PageIndex,
                pagedRequest.PageSize,
                pagedRequest.OrderBy
            );

            // Chuyển đổi danh sách dịch vụ sang kiểu ServiceResponse
            var serviceResponses = services?.Select(s => _mapper.Map<ServiceResponse>(s)) ?? Enumerable.Empty<ServiceResponse>();

            // Tạo đối tượng PagedResponse để trả về
            var pagedResponse = new PagedResponse<ServiceResponse>(
                serviceResponses,
                totalCount,
                pagedRequest.PageIndex,
                pagedRequest.PageSize
            );

            // Trả về kết quả thành công với thông báo và dữ liệu đã phân trang
            return Ok(new ResponseObject<PagedResponse<ServiceResponse>>("Lấy dịch vụ thành công", pagedResponse));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceByIdAync(int id)
        {
            try
            {
                var service = await _serviceService.GetServiceByIdAsync(id);
                if (service == null)
                {
                    return NotFound(new ResponseObject<ServiceResponse>($"Dịch vụ với ID {id} không tìm thấy.", null));
                }

                // Ánh xạ Service sang ServiceResponse
                var response = _mapper.Map<ServiceResponse>(service);

                return Ok(new ResponseObject<ServiceResponse>("Lấy dịch vụ thành công", response));
            }
            catch (Exception ex)
            {
                return NotFound(new ResponseObject<string>(ex.Message));
            }
        }

        [HttpGet("business/{id}")]
        public async Task<IActionResult> GetServiceByBusinessId([FromQuery] PagedRequest request, int id)
        {
            if (request.PageIndex == 0 || request.PageSize == 0)
            {
                request.PageIndex = 1;
                request.PageSize = 10;
            }
            var (services, total) = await _serviceService.GetServicesByBusinessIdAsync(id, request.Keyword, request.Status, request.PageIndex, request.PageSize, request.OrderBy);
            var serviceResponses = services?.Select(s => _mapper.Map<ServiceResponse>(s)) ?? Enumerable.Empty<ServiceResponse>();
            var pagedResponse = new PagedResponse<ServiceResponse>(
                    serviceResponses,
                    total,
                    request.PageIndex,
                    request.PageSize
                );
            return Ok(new ResponseObject<PagedResponse<ServiceResponse>>("", pagedResponse));
        }
        [HttpGet("categories/{id}")]
        public async Task<IActionResult> GetServiceByCategoryId([FromQuery] PagedRequest request, int id)
        {
            if (request.PageIndex == 0 || request.PageSize == 0)
            {
                request.PageIndex = 1;
                request.PageSize = 10;
            }
            var (services, total) = await _serviceService.GetServicesByCategoryIdAsync(id, request.Keyword, request.Status, request.PageIndex, request.PageSize, request.OrderBy);
            var serviceResponses = services?.Select(s => _mapper.Map<ServiceResponse>(s)) ?? Enumerable.Empty<ServiceResponse>();
            var pagedResponse = new PagedResponse<ServiceResponse>(
                    serviceResponses,
                    total,
                    request.PageIndex,
                    request.PageSize
                );
            return Ok(new ResponseObject<PagedResponse<ServiceResponse>>("", pagedResponse));
        }
        [HttpGet("discounted")]
        public async Task<IActionResult> GetDiscountedServicesAsync([FromQuery] PagedRequest request)
        {
            try
            {
                if (request.PageIndex == 0 || request.PageSize == 0)
                {
                    request.PageIndex = 1;
                    request.PageSize = 10;
                }
                var (discountedServices, totalCount) = await _serviceService.GetDiscountedServicesIncludeBusinessRankAsync(request.Keyword, request.Status, request.PageIndex, request.PageSize);

                if (discountedServices == null || !discountedServices.Any())
                {
                    return NotFound(new ResponseObject<IEnumerable<ServiceResponseV3>>("Không tìm thấy dịch vụ nào đang giảm giá."));
                }

                var pagedResponse = new PagedResponse<ServiceResponseV3>(
                    discountedServices ?? Enumerable.Empty<ServiceResponseV3>(),
                    totalCount,
                    request.PageIndex,
                    request.PageSize
                );

                return Ok(new ResponseObject<PagedResponse<ServiceResponseV3>>("Lấy dịch vụ giảm giá thành công.", pagedResponse));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>("Đã xảy ra lỗi khi lấy dịch vụ.", ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateServiceAsync([FromBody] ServiceCreateRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Status) || !Util.IsValidGeneralStatus(request.Status))
                {
                    throw new ArgumentException("Trạng thái của dịch vụ không hợp lệ.", nameof(request.Status));
                }
                request.Status = request.Status.Trim().ToUpperInvariant();

                string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int.TryParse(userIdClaim, out int id);

                var businessId = await _businessService.GetBusinessIdByOwnerIdAsync(id);

                if (businessId == null)
                {
                    throw new ArgumentException("Account này chưa có doanh nghiệp nào.");
                }

                await _serviceService.AddServiceAsync(request, businessId.Value);
                return Ok(new ResponseObject<string>("Tạo dịch vụ thành công"));
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new ResponseObject<string>($"Lỗi: {ex.Message}", null));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseObject<string>($"Lỗi xác thực: {ex.Message}", null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Đã xảy ra lỗi không mong muốn: {ex.Message}", null));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServiceAsync([FromBody] ServiceUpdateRequest serviceUpdateRequest, int id)
        {
            if (serviceUpdateRequest == null)
            {
                return BadRequest("Yêu cầu cập nhật dịch vụ không được để trống.");
            }

            try
            {
                await _serviceService.UpdateServiceAsync(serviceUpdateRequest, id);
                return Ok("Các dịch vụ đã được cập nhật thành công.");
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi máy chủ nội bộ: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceAync(int id)
        {
            try
            {
                await _serviceService.DeleteServiceAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new ResponseObject<ServiceResponse>(ex.Message, null));
            }
        }
    }
}
