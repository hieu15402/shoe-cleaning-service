using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TP4SCS.Library.Models.Request.Account;
using TP4SCS.Library.Models.Request.BusinessProfile;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        //[Authorize(Policy = "Admin")]
        [HttpGet]
        [Route("api/accounts")]
        public async Task<IActionResult> GetAccountsAsync([FromQuery] GetAccountRequest getAccountRequest)
        {
            var result = await _accountService.GetAccountsAsync(getAccountRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        //[Authorize(Policy = "Admin")]
        [HttpGet]
        [Route("api/accounts/employees")]
        public async Task<IActionResult> GetAccountsAsync([FromQuery] GetEmployeeRequest getEmployeeRequest)
        {
            var result = await _accountService.GetEmployeesAsync(getEmployeeRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        [Route("api/accounts/{id}", Name = "GetAccountById")]
        public async Task<IActionResult> GetAccountByIdAsync([FromRoute] int id)
        {
            var result = await _accountService.GetAccountByIdAsync(id);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        //[Authorize(Policy = "Admin")]
        [HttpPost]
        [Route("api/accounts/moderator")]
        public async Task<IActionResult> CreateModeratorAccountAsync([FromBody] CreateModeratorRequest createModeratorRequest)
        {
            var result = await _accountService.CreateModeratorAccountAsync(createModeratorRequest);

            if (result.StatusCode != 201)
            {
                return StatusCode(result.StatusCode, result);
            }

            int newAccId = await _accountService.GetAccountMaxIdAsync();

            return StatusCode(result.StatusCode, result);
        }

        //[Authorize(Policy = "Owner")]
        [HttpPost]
        [Route("api/accounts/employee")]
        public async Task<IActionResult> CreateEmployeeAccountAsync([FromBody] CreateEmployeeRequest createEmployeeRequest)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var ownerId = int.TryParse(userIdClaim, out int id);

            var result = await _accountService.CreateEmployeeAccountAsync(id, createEmployeeRequest);

            if (result.StatusCode != 201)
            {
                return StatusCode(result.StatusCode, result);
            }

            int newAccId = await _accountService.GetAccountMaxIdAsync();

            return StatusCode(result.StatusCode, result);
        }

        [Authorize]
        [HttpPut]
        [Route("api/accounts/{id}")]
        public async Task<IActionResult> UpdateAccountAsync([FromRoute] int id, UpdateAccountRequest updateAccountRequest)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !userIdClaim.Equals(id.ToString()))
            {
                return Forbid();
            }

            var result = await _accountService.UpdateAccountAsync(id, updateAccountRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [Authorize(Policy = "Customer")]
        [HttpPut]
        [Route("api/accounts/{id}/become-owner")]
        public async Task<IActionResult> UpdateAccountToOwnerAsync([FromRoute] int id, CreateBusinessRequest createBusinessRequest)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !userIdClaim.Equals(id.ToString()))
            {
                return Forbid();
            }

            var result = await _accountService.UpdateAccountToOwnerAsync(id, createBusinessRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPut]
        [Route("api/accounts/{id}/password")]
        public async Task<IActionResult> UpdateAccountPasswordAsync([FromRoute] int id, UpdateAccountPasswordRequest updateAccountPasswordRequest)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null || !userIdClaim.Equals(id.ToString()))
            {
                return Forbid();
            }

            var result = await _accountService.UpdateAccountPasswordAsync(id, updateAccountPasswordRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [Authorize(Policy = "Admin")]
        [HttpPut]
        [Route("api/accounts/{id}/status")]
        public async Task<IActionResult> UpdateAccountStatusForAdminAsync([FromRoute] int id, [FromBody] UpdateStatusRequest updateStatusRequest)
        {
            var result = await _accountService.UpdateAccountStatusForAdminAsync(id, updateStatusRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpDelete]
        [Route("api/accounts/{id}")]
        public async Task<IActionResult> DeleteAccountAsync([FromRoute] int id)
        {
            var result = await _accountService.DeleteAccountAsync(id);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }
    }
}
