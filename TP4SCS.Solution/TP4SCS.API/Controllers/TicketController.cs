using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TP4SCS.Library.Models.Request.Ticket;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [Route("api/support-tickets")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _ticketService;

        public TicketController(ITicketService ticketService)
        {
            _ticketService = ticketService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetTicketsAsync([FromQuery] GetTicketRequest getTicketRequest)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string? userRole = HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;

            var userId = int.TryParse(userIdClaim, out int id);

            if (userRole != null &&
                !userRole.Equals(RoleConstants.ADMIN) &&
                !userRole.Equals(RoleConstants.MODERATOR) &&
                getTicketRequest.AccountId.HasValue &&
                id != getTicketRequest.AccountId)
            {
                return Forbid();
            }

            var result = await _ticketService.GetTicketsAsync(getTicketRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [Authorize(Policy = "Employee")]
        [HttpGet("branch/{id}")]
        public async Task<IActionResult> GetTicketsByBranchIdAsync([FromRoute] int id, [FromQuery] GetBusinessTicketRequest getBusinessTicketRequest)
        {
            var result = await _ticketService.GetTicketsByBranchIdAsync(id, getBusinessTicketRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [Authorize(Policy = "Employee")]
        [HttpGet("business/{id}")]
        public async Task<IActionResult> GetTicketsByBusinessIdAsync([FromRoute] int id, [FromQuery] GetBusinessTicketRequest getBusinessTicketRequest)
        {
            var result = await _ticketService.GetTicketsByBusinessAsync(id, getBusinessTicketRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketByIdAsync([FromRoute] int id)
        {
            var result = await _ticketService.GetTicketByIdAsync(id);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateTicketAsync([FromBody] CreateTicketRequest createTicketRequest)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var userId = int.TryParse(userIdClaim, out int id);

            var result = await _ticketService.CreateTicketAsync(id, createTicketRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost("order-ticket")]
        public async Task<IActionResult> CreateOrderTicketAsync([FromBody] CreateOrderTicketRequest createOrderTicketRequest)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var userId = int.TryParse(userIdClaim, out int id);

            var result = await _ticketService.CreateOrderTicketAsync(id, createOrderTicketRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPost("{id}/child-ticket")]
        public async Task<IActionResult> CreateChildTicketAsync([FromRoute] int id, [FromBody] CreateChildTicketRequest createChildTicketRequest)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var accId = int.TryParse(userIdClaim, out int userid);

            var result = await _ticketService.CreateChildTicketAsync(userid, id, createChildTicketRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [Authorize(Policy = "Moderator")]
        [HttpPost("notify-for-customer")]
        public async Task<IActionResult> NotifyForCustomerAsync([FromBody] NotifyTicketForCustomerRequest notifyTicketForCustomerRequest)
        {
            var result = await _ticketService.NotifyForCustomerAsync(notifyTicketForCustomerRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [Authorize(Policy = "Moderator")]
        [HttpPost("notify-for-owner")]
        public async Task<IActionResult> NotifyForOwnerAsync([FromBody] NotifyTicketForOwnerRequest notifyTicketForOwnerRequest)
        {
            var result = await _ticketService.NotifyForOwnerAsync(notifyTicketForOwnerRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        //[Authorize(Policy = "Moderator")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateTicketStatusAsync([FromRoute] int id, [FromBody] UpdateTicketStatusRequest updateTicketStatusRequest)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var userId = int.TryParse(userIdClaim, out int morderatorId);

            var result = await _ticketService.UpdateTicketStatusAsync(morderatorId, id, updateTicketStatusRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [Authorize]
        [HttpPut("{id}/cancel")]
        public async Task<IActionResult> CancelTicketAsync([FromRoute] int id)
        {
            var result = await _ticketService.CancelTicketAsync(id);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }
    }
}
