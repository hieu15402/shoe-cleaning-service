using Microsoft.AspNetCore.Mvc;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [ApiController]
    public class LeaderboardController : ControllerBase
    {
        private readonly ILeaderboardService _leaderboardService;

        public LeaderboardController(ILeaderboardService leaderboardService)
        {
            _leaderboardService = leaderboardService;
        }

        [HttpGet]
        [Route("api/leaderboards/by-month")]
        public async Task<IActionResult> GetLeaderboardByMonthAsync()
        {
            var result = await _leaderboardService.GetLeaderboardByMonthAsync();

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("api/leaderboards/by-year")]
        public async Task<IActionResult> GetLeaderboardByYearAsync()
        {
            var result = await _leaderboardService.GetLeaderboardByMonthAsync();

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpPut]
        [Route("api/leaderboards")]
        public async Task<IActionResult> UpdateLeaderboardAsync()
        {
            var result = await _leaderboardService.UpdateLeaderboardAsync();

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }
    }
}
