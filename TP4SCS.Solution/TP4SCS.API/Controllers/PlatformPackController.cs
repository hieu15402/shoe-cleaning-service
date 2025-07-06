using Microsoft.AspNetCore.Mvc;
using TP4SCS.Library.Models.Request.PlatformPack;
using TP4SCS.Library.Models.Request.SubscriptionPack;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [Route("api/platform-packs")]
    [ApiController]
    public class PlatformPackController : ControllerBase
    {
        private readonly IPlatformPackService _subscriptionService;

        public PlatformPackController(IPlatformPackService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet("register-packs")]
        public async Task<IActionResult> GetRegisterPacksAsync()
        {
            var result = await _subscriptionService.GetRegisterPacksAsync();

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpGet("feature-packs")]
        public async Task<IActionResult> GetFeaturePacksAsync()
        {
            var result = await _subscriptionService.GetFeaturePacksAsync();

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPackByIdAsync([FromRoute] int id)
        {
            var result = await _subscriptionService.GetPackByIdAsync(id);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpPost("register-pack")]
        public async Task<IActionResult> CreatePackAsync([FromBody] RegisterPackRequest registerPackRequest)
        {
            var result = await _subscriptionService.CreateRegisterPackAsync(registerPackRequest);

            if (result.StatusCode != 201)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpPut("{id}/register-pack")]
        public async Task<IActionResult> UpdateRegisterPackAsync([FromRoute] int id, [FromBody] RegisterPackRequest registerPackRequest)
        {
            var result = await _subscriptionService.UpdateRegisterPackAsync(id, registerPackRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpPut("{id}/feature-pack")]
        public async Task<IActionResult> UpdateRegisterPackAsync([FromRoute] int id, [FromBody] FeaturePackRequest featurePackRequest)
        {
            var result = await _subscriptionService.UpdateFeaturePackAsync(id, featurePackRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpDelete("{id}/register-pack")]
        public async Task<IActionResult> DeleteRegisterPackAsync([FromRoute] int id)
        {
            var result = await _subscriptionService.DeleteRegisterPackAsync(id);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }
    }
}
