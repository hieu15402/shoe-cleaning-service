using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Library.Models.Request.Material;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.Material;
using TP4SCS.Library.Utils.Utils;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [Route("api/materials")]
    [ApiController]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _materialService;
        private readonly IMapper _mapper;
        private readonly IBusinessService _businessService;

        public MaterialController(IMaterialService materialService, IMapper mapper, IBusinessService businessService)
        {
            _materialService = materialService;
            _mapper = mapper;
            _businessService = businessService;
        }

        [HttpGet]
        public async Task<IActionResult> GetMaterialsAsync(
            [FromQuery] string? keyword = null,
            [FromQuery] string? status = null,
            [FromQuery] OrderByEnum orderBy = OrderByEnum.IdDesc,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var (materials, total) = await _materialService.GetMaterialsAsync(keyword, status, orderBy, pageIndex, pageSize);

                if (materials == null || !materials.Any())
                {
                    var emptyResponse = new PagedResponse<MaterialResponse>(new List<MaterialResponse>(), 0, pageIndex, pageSize);
                    var response = new ResponseObject<PagedResponse<MaterialResponse>>("No materials found.", emptyResponse);
                    return Ok(response);
                }

                var materialResponses = _mapper.Map<IEnumerable<MaterialResponse>>(materials);

                var pagedResponse = new PagedResponse<MaterialResponse>(materialResponses, total, pageIndex, pageSize);

                var successResponse = new ResponseObject<PagedResponse<MaterialResponse>>("Materials retrieved successfully.", pagedResponse);

                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ResponseObject<PagedResponse<MaterialResponse>>($"Error: {ex.Message}", null);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMaterialsByIdAsync(int id)
        {
            try
            {
                var material = await _materialService.GetMaterialByIdAsync(id);
                var materialResponse = _mapper.Map<MaterialResponse>(material);

                return Ok(new ResponseObject<MaterialResponse>("Material retrieved successfully.", materialResponse));
            }
            catch (Exception ex)
            {
                var errorResponse = new ResponseObject<PagedResponse<MaterialResponse>>($"Error: {ex.Message}", null);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpGet("branches/{branchId}")]
        public async Task<IActionResult> GetMaterialsByBranchIdAsync(
            int branchId,
            [FromQuery] string? keyword = null,
            [FromQuery] string? status = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] OrderByEnum orderBy = OrderByEnum.IdDesc)
        {
            try
            {
                var (materials, total) = await _materialService.GetMaterialsByBranchIdAsync(branchId, keyword, status, pageIndex, pageSize, orderBy);

                if (materials == null || !materials.Any())
                {
                    var emptyResponse = new PagedResponse<MaterialResponse>(new List<MaterialResponse>(), 0, pageIndex, pageSize);
                    var response = new ResponseObject<PagedResponse<MaterialResponse>>("No materials found for this branch.", emptyResponse);
                    return Ok(response);
                }

                var materialResponses = _mapper.Map<IEnumerable<MaterialResponse>>(materials);

                var pagedResponse = new PagedResponse<MaterialResponse>(materialResponses, total, pageIndex, pageSize);

                var successResponse = new ResponseObject<PagedResponse<MaterialResponse>>("Materials retrieved successfully.", pagedResponse);

                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ResponseObject<PagedResponse<MaterialResponse>>($"Error: {ex.Message}", null);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpGet("businesses/{businessId}")]
        public async Task<IActionResult> GetMaterialsByBusinessIdAsync(
            int businessId,
            [FromQuery] string? keyword = null,
            [FromQuery] string? status = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] OrderByEnum orderBy = OrderByEnum.IdDesc)
        {
            try
            {
                var (materials, total) = await _materialService.GetMaterialsByBusinessIdAsync(businessId, keyword, status, pageIndex, pageSize, orderBy);

                if (materials == null || !materials.Any())
                {
                    var emptyResponse = new PagedResponse<MaterialResponse>(new List<MaterialResponse>(), 0, pageIndex, pageSize);
                    var response = new ResponseObject<PagedResponse<MaterialResponse>>("No materials found for this business.", emptyResponse);
                    return Ok(response);
                }

                var materialResponses = _mapper.Map<IEnumerable<MaterialResponse>>(materials);

                var pagedResponse = new PagedResponse<MaterialResponse>(materialResponses, total, pageIndex, pageSize);

                var successResponse = new ResponseObject<PagedResponse<MaterialResponse>>("Materials retrieved successfully.", pagedResponse);

                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ResponseObject<PagedResponse<MaterialResponse>>($"Error: {ex.Message}", null);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpGet("services/{id}")]
        public async Task<IActionResult> GetMaterialsByServiceIdAsync(
            int id,
            [FromQuery] string? keyword = null,
            [FromQuery] string? status = null,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] OrderByEnum orderBy = OrderByEnum.IdDesc)
        {
            try
            {
                var (materials, total) = await _materialService.GetMaterialsByServiceIdAsync(id, keyword, status, pageIndex, pageSize, orderBy);

                if (materials == null || !materials.Any())
                {
                    var emptyResponse = new PagedResponse<MaterialResponse>(new List<MaterialResponse>(), 0, pageIndex, pageSize);
                    var response = new ResponseObject<PagedResponse<MaterialResponse>>("No materials found for this business.", emptyResponse);
                    return Ok(response);
                }

                var materialResponses = _mapper.Map<IEnumerable<MaterialResponse>>(materials);

                var pagedResponse = new PagedResponse<MaterialResponse>(materialResponses, total, pageIndex, pageSize);

                var successResponse = new ResponseObject<PagedResponse<MaterialResponse>>("Materials retrieved successfully.", pagedResponse);

                return Ok(successResponse);
            }
            catch (Exception ex)
            {
                var errorResponse = new ResponseObject<PagedResponse<MaterialResponse>>($"Error: {ex.Message}", null);
                return StatusCode(500, errorResponse);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateMaterialAsync([FromBody] MaterialCreateRequest request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Status) || !Util.IsValidGeneralStatus(request.Status))
                {
                    throw new ArgumentException("Material status is invalid.", nameof(request.Status));
                }
                request.Status = request.Status.Trim().ToUpperInvariant();

                string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int.TryParse(userIdClaim, out int id);

                var businessId = await _businessService.GetBusinessIdByOwnerIdAsync(id);

                if (businessId == null)
                {
                    throw new ArgumentException("This account has no business.");
                }

                await _materialService.AddMaterialAsync(request, businessId.Value);
                return Ok(new ResponseObject<string>("Material created successfully"));
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new ResponseObject<MaterialResponse>($"Error: {ex.Message}", null));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseObject<MaterialResponse>($"Validation error: {ex.Message}", null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<MaterialResponse>($"Unexpected error: {ex.Message}", null));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateMaterialAsync([FromBody] MaterialUpdateRequest materialUpdateRequest, int id)
        {
            if (materialUpdateRequest == null)
            {
                return BadRequest("Material update request cannot be empty.");
            }

            try
            {
                await _materialService.UpdateMaterialAsync(materialUpdateRequest, id);
                return Ok("Material updated successfully.");
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
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("update-quantity")]
        public async Task<IActionResult> UpdateMaterialQuantityAsync([FromBody] int quantity, [FromQuery] int branchId, [FromQuery] int materialId)
        {
            if (quantity <= 0)
            {
                return BadRequest("Quantity must be greater than zero.");
            }

            try
            {
                await _materialService.UpdateMaterialAsync(quantity, branchId, materialId);
                return Ok(new ResponseObject<string>("Material quantity updated successfully."));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseObject<string>($"Material not found: {ex.Message}", null));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Internal server error: {ex.Message}", null));
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMaterialAsync(int id)
        {
            try
            {
                await _materialService.DeleteMaterialAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new ResponseObject<MaterialResponse>(ex.Message, null));
            }
        }
    }
}