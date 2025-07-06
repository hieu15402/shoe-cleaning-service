using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TP4SCS.Library.Models.Request.Business;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [ApiController]
    public class BusinessController : ControllerBase
    {
        private readonly IBusinessService _businessService;

        public BusinessController(IBusinessService businessService)
        {
            _businessService = businessService;
        }

        [HttpGet]
        [Route("api/businesses")]
        public async Task<IActionResult> GetBusinessProfilesAsync([FromQuery] GetBusinessRequest getBusinessRequest)
        {
            var result = await _businessService.GetBusinessesProfilesAsync(getBusinessRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("api/businesses/{id}", Name = "GetBusinessProfileById")]
        public async Task<IActionResult> GetBusinessProfileByIdAsync([FromRoute] int id)
        {
            var result = await _businessService.GetBusinessProfileByIdAsync(id);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpPut]
        [Route("api/businesses/{id}")]
        public async Task<IActionResult> UpdateBusinessProfileAsync([FromRoute] int id, [FromBody] UpdateBusinessRequest updateBusinessRequest)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var userId = int.TryParse(userIdClaim, out int ownerId);

            if (!await _businessService.CheckOwnerOfBusiness(ownerId, id))
            {
                Forbid();
            }

            var result = await _businessService.UpdateBusinessProfileAsync(id, updateBusinessRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [Authorize(Policy = "Admin")]
        [HttpPut]
        [Route("api/businesses/{id}/status")]
        public async Task<IActionResult> UpdateBusinessStatusForAdminAsync([FromRoute] int id, [FromBody] UpdateBusinessStatusRequest updateBusinessStatusRequest)
        {
            var result = await _businessService.UpdateBusinessStatusForAdminAsync(id, updateBusinessStatusRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        //[Authorize(Policy = "Moderator")]
        //[HttpPut]
        //[Route("api/businesses/{id}/validate-buisness")]
        //public async Task<IActionResult> UpdateBusinessStatusForAdminAsync([FromRoute] int id, [FromBody] ValidateBusinessRequest validateBusinessRequest)
        //{
        //    var result = await _businessService.ValidateBusinessAsync(id, validateBusinessRequest);

        //    if (result.StatusCode != 200)
        //    {
        //        return StatusCode(result.StatusCode, result);
        //    }

        //    return Ok(result);
        //}
    }
}
