using Microsoft.AspNetCore.Mvc;
using TP4SCS.Library.Models.Request.Category;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [Route("api/ticket-categories")]
    [ApiController]
    public class TicketCategoryController : ControllerBase
    {
        private readonly ITicketCategoryService _ticketCategoryService;

        public TicketCategoryController(ITicketCategoryService ticketCategoryService)
        {
            _ticketCategoryService = ticketCategoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoriesAsync()
        {
            var result = await _ticketCategoryService.GetCategoriesAsync();

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById([FromRoute] int id)
        {
            var result = await _ticketCategoryService.GetCategoryByIdAsync(id);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] TicketCategoryRequest ticketCategoryRequest)
        {
            var result = await _ticketCategoryService.CreateCategoryAsync(ticketCategoryRequest);

            if (result.StatusCode != 201)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory([FromRoute] int id, [FromBody] UpdateTicketCategoryRequest updateTicketCategoryRequest)
        {
            var result = await _ticketCategoryService.UpdateCategoryAsync(id, updateTicketCategoryRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }
    }
}
