using Microsoft.AspNetCore.Mvc;
using TP4SCS.Library.Models.Request.PlatformStatistic;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [Route("api/platform-statistics")]
    [ApiController]
    public class PlatformStatisticController : ControllerBase
    {
        private readonly IPlatformStatisticService _platformStatisticService;

        public PlatformStatisticController(IPlatformStatisticService platformStatisticService)
        {
            _platformStatisticService = platformStatisticService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPlatformStatisticAsync([FromQuery] GetPlatformStatisticRequest getPlatformStatisticRequest)
        {
            var result = await _platformStatisticService.GetPlatformStatisticAsync(getPlatformStatisticRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpGet("user-statistic")]
        public async Task<IActionResult> GetPlatformUserStatisticAsync()
        {
            var result = await _platformStatisticService.GetPlatformUserStatisticAsync();

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePlatformStatisticAsync([FromQuery] UpdatePlatformStatisticRequest updatePlatformStatisticRequest)
        {
            var result = await _platformStatisticService.UpdatePlatformStatisticAsync(updatePlatformStatisticRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }
    }
}
