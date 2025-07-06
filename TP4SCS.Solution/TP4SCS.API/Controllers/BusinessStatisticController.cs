using Microsoft.AspNetCore.Mvc;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [Route("api/business-statistics")]
    [ApiController]
    public class BusinessStatisticController : ControllerBase
    {
        private readonly IBusinessStatisticService _businessStatisticService;

        public BusinessStatisticController(IBusinessStatisticService businessStatisticService)
        {
            _businessStatisticService = businessStatisticService;
        }

        [HttpGet("{businessId}/order-by-month")]
        public async Task<IActionResult> GetBusinessOrderStatisticByMonthAsync([FromRoute] int businessId)
        {
            var result = await _businessStatisticService.GetBusinessOrderStatisticByMonthAsync(businessId);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpGet("{businessId}/order-by-year")]
        public async Task<IActionResult> GetBusinessOrderStatisticByYearAsync([FromRoute] int businessId)
        {
            var result = await _businessStatisticService.GetBusinessOrderStatisticByYearAsync(businessId);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpGet("{businessId}/feedback-by-month")]
        public async Task<IActionResult> GetBusinessFeedbackStatisticByMonthAsync([FromRoute] int businessId)
        {
            var result = await _businessStatisticService.GetBusinessFeedbackStatisticByMonthAsync(businessId);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpGet("{businessId}/feedback-by-year")]
        public async Task<IActionResult> GetBusinessFeedbackStatisticByYearAsync([FromRoute] int businessId)
        {
            var result = await _businessStatisticService.GetBusinessFeedbackStatisticByYearAsync(businessId);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpGet("{businessId}/profit-by-month")]
        public async Task<IActionResult> GetBusinessProfitStatisticByMonthAsync([FromRoute] int businessId)
        {
            var result = await _businessStatisticService.GetBusinessProfitStatisticByMonthAsync(businessId);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpGet("{businessId}/profit-by-year")]
        public async Task<IActionResult> GetBusinessProfitStatisticByYearAsync([FromRoute] int businessId)
        {
            var result = await _businessStatisticService.GetBusinessProfitStatisticByYearAsync(businessId);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpPut("{businessId}/order")]
        public async Task<IActionResult> UpdateBusinessOrderStatisticAsync([FromRoute] int businessId)
        {
            var result = await _businessStatisticService.UpdateBusinessOrderStatisticAsync(businessId);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpPut("{businessId}/feedback")]
        public async Task<IActionResult> UpdateBusinessFeedbackStatisticAsync([FromRoute] int businessId)
        {
            var result = await _businessStatisticService.UpdateBusinessFeedbackStatisticAsync(businessId);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpPut("{businessId}/profit")]
        public async Task<IActionResult> UpdateBusinessProfitStatisticAsync([FromRoute] int businessId)
        {
            var result = await _businessStatisticService.UpdateBusinessProfitStatisticAsync(businessId);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }
    }
}
