using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Category;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Library.Models.Response.Category;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Utils.Utils;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class ServiceCategoryController : ControllerBase
    {
        private IServiceCategoryService _categoryService;
        private IMapper _mapper;

        public ServiceCategoryController(IServiceCategoryService categoryService, IMapper mapper)
        {
            _categoryService = categoryService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategoriesAsync([FromQuery] PagedRequest pagedRequest)
        {
            var services = await _categoryService.GetServiceCategoriesAsync(pagedRequest.Keyword,
                pagedRequest.Status,
                pagedRequest.PageIndex, pagedRequest.PageSize, pagedRequest.OrderBy);
            var totalCount = await _categoryService.GetTotalServiceCategoriesCountAsync(pagedRequest.Keyword, pagedRequest.Status);

            var pagedResponse = new PagedResponse<ServiceCategoryResponse>(
                services?.Select(s =>
                {
                    var serviceCategoryResponse = _mapper.Map<ServiceCategoryResponse>(s);
                    serviceCategoryResponse.Status = Util.TranslateGeneralStatus(s.Status);
                    return serviceCategoryResponse;
                }) ?? Enumerable.Empty<ServiceCategoryResponse>(),
                totalCount,
                pagedRequest.PageIndex,
                pagedRequest.PageSize
            );

            return Ok(new ResponseObject<PagedResponse<ServiceCategoryResponse>>("Fetch Category Success", pagedResponse));
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryByIdAync(int id)
        {
            try
            {
                var category = await _categoryService.GetServiceCategoryByIdAsync(id);
                if (category == null)
                {
                    Ok(new ResponseObject<ServiceCategoryResponse>($"Category with ID {id} not found.", null));
                }
                var response = _mapper.Map<ServiceCategoryResponse>(category);
                return Ok(new ResponseObject<ServiceCategoryResponse>("Fetch Category Success", response));
            }
            catch (Exception ex)
            {
                return NotFound(new ResponseObject<string>(ex.Message));
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategoryAync([FromBody] ServiceCategoryRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ResponseObject<ServiceCategoryResponse>("Request body cannot be null."));
            }

            try
            {
                var category = _mapper.Map<ServiceCategory>(request);
                await _categoryService.AddServiceCategoryAsync(category);
                return CreatedAtAction(nameof(GetCategoryByIdAync), new { id = category.Id },
                    new ResponseObject<string>("Create Category Success"));
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new ResponseObject<string>(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseObject<string>(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"An error occurred: {ex.Message}"));
            }
        }

        [HttpPut("{existingCategoryId}")]
        public async Task<IActionResult> UpdateCategoryAync(int existingCategoryId, [FromBody] ServiceCategoryRequest request)
        {
            if (request == null)
            {
                return BadRequest(new ResponseObject<ServiceCategoryResponse>("Request body cannot be null."));
            }

            try
            {
                var categoryToUpdate = _mapper.Map<ServiceCategory>(request);

                await _categoryService.UpdateServiceCategoryAsync(categoryToUpdate, existingCategoryId);

                return Ok(new ResponseObject<string>("Update Category Success"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseObject<string>(ex.Message));
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new ResponseObject<string>(ex.Message));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseObject<string>(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"An error occurred: {ex.Message}"));
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategoryAync(int id)
        {
            try
            {
                await _categoryService.DeleteServiceCategoryAsync(id);

                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseObject<string>(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"An error occurred: {ex.Message}"));
            }
        }


    }
}
