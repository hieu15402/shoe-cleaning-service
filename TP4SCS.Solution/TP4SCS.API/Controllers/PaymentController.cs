using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TP4SCS.Library.Models.Request.Payment;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost("payment-url")]
        public async Task<IActionResult> CreatePaymentUrlAsync(PaymentRequest paymentRequest)
        {
            string? userIdClaim = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var userId = int.TryParse(userIdClaim, out int id);

            var result = await _paymentService.CreatePaymentUrlAsync(HttpContext, id, paymentRequest);

            if (result.StatusCode != 201)
            {
                return StatusCode(result.StatusCode, result);
            }

            if (!Uri.TryCreate(result.Data, UriKind.Absolute, out Uri? link))
            {
                return BadRequest("Định Dạng URL Không Hợp Lệ!");
            }

            return Ok(link.ToString());
        }

        [HttpGet("VnPay")]
        public async Task<IActionResult> ExcuteVnPayAsync()
        {
            var result = await _paymentService.VnPayExcuteAsync(HttpContext.Request.Query);

            if (result.StatusCode != 200)
            {
                return Redirect("https://www.shoecarehub.xyz/owner/payments/fail");
            }

            return Redirect("https://www.shoecarehub.xyz/owner/payments/success");
        }

        [HttpGet("MoMo")]
        public async Task<IActionResult> ExcuteMoMoAsync()
        {
            var result = await _paymentService.MoMoExcuteAsync(HttpContext.Request.Query);

            if (result.StatusCode != 200)
            {
                return Redirect("https://www.shoecarehub.xyz/owner/payments/fail");
            }

            return Redirect("https://www.shoecarehub.xyz/owner/payments/success");
        }
    }
}
