using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TP4SCS.Library.Models.Request.Branch;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IBusinessBranchService _branchService;
        private readonly IBusinessService _businessService;
        private readonly HttpClient _httpClient;

        public BranchController(IBusinessBranchService branchService, IBusinessService businessService, HttpClient httpClient)
        {
            _branchService = branchService;
            _businessService = businessService;
            _httpClient = httpClient;
        }

        [HttpGet]
        [Route("api/branches/business/{id}")]
        public async Task<IActionResult> GetBranchByBusinessIdAsync([FromRoute] int id)
        {
            var result = await _branchService.GetBranchesByBusinessIdAsync(id);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("api/branches/{id}", Name = "GetBranchById")]
        public async Task<IActionResult> GetBranchByIdAsync([FromRoute] int id)
        {
            var result = await _branchService.GetBranchByIdAsync(id);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [Authorize(Policy = "Owner")]
        [HttpPost]
        [Route("api/branches")]
        public async Task<IActionResult> CreateBranchAsync([FromBody] CreateBranchRequest createBranchRequest)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var userId = int.TryParse(userIdClaim, out int id);

            var result = await _branchService.CreateBranchByOwnerIdAsync(id, _httpClient, createBranchRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            var newBrId = await _branchService.GetBranchMaxIdAsync();

            return CreatedAtAction("GetBranchById", new { id = newBrId }, result.Data);

        }

        [Authorize(Policy = "Owner")]
        [HttpPut]
        [Route("api/branches/{id}")]
        public async Task<IActionResult> UpdateBranchAsync([FromRoute] int id, [FromBody] UpdateBranchRequest updateBranchRequest)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var userId = int.TryParse(userIdClaim, out int ownerId);

            if (!await _branchService.CheckOwnerOfBranch(ownerId, id))
            {
                return Forbid();
            }

            var result = await _branchService.UpdateBranchAsync(id, _httpClient, updateBranchRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [Authorize(Policy = "Owner")]
        [HttpPut]
        [Route("api/branches/{id}/employee")]
        public async Task<IActionResult> UpdateBranchEmployeeAsync([FromRoute] int id, [FromBody] UpdateBranchEmployeeRequest updateBranchEmployeeRequest)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var userId = int.TryParse(userIdClaim, out int ownerId);

            if (!await _branchService.CheckOwnerOfBranch(ownerId, id))
            {
                return Forbid();
            }

            var result = await _branchService.UpdateBranchEmployeeAsync(id, updateBranchEmployeeRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [Authorize(Policy = "Admin")]
        [HttpPut]
        [Route("api/branches/{id}/status")]
        public async Task<IActionResult> UpdateBranchStatusAsync([FromRoute] int id, [FromBody] UpdateBranchStatusRequest updateBranchStatusRequest)
        {
            var result = await _branchService.UpdateBranchStatusForAdminAsync(id, updateBranchStatusRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }
    }
}
