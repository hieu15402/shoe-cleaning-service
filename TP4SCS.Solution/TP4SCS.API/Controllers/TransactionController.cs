using Microsoft.AspNetCore.Mvc;
using TP4SCS.Library.Models.Request.Transaction;
using TP4SCS.Library.Services;

namespace TP4SCS.WebAPI.Controllers
{
    [Route("api/transactions")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactionsAsync([FromQuery] GetTransactionRequest getTransactionRequest)
        {
            var result = await _transactionService.GetTransactionsAsync(getTransactionRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTransactionByIdAsync([FromRoute] int id)
        {
            var result = await _transactionService.GetTransactionByIdAsync(id);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTransactionAsync([FromBody] CreateTransactionRequest createTransactionRequest)
        {
            var result = await _transactionService.CreateTransactionAsync(createTransactionRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransactionAsync([FromRoute] int id, [FromBody] UpdateTransactionRequest updateTransactionRequest)
        {
            var result = await _transactionService.UpdateTransactionAsync(id, updateTransactionRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransactionAsync([FromRoute] int id)
        {
            var result = await _transactionService.DeteleTransactionAsync(id);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }
    }
}
