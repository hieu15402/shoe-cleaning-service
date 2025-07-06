using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Library.Models.Request.Promotion;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.Promotion;
using TP4SCS.Library.Utils.Utils;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [ApiController]
    [Route("api/promotions")]
    public class PromotionController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IPromotionService _promotionService;

        public PromotionController(IMapper mapper, IPromotionService promotionService)
        {
            _mapper = mapper;
            _promotionService = promotionService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPromotionsAsync([FromQuery] PagedRequest pagedRequest)
        {
            var promotions = await _promotionService.GetPromotionsAsync(
                pagedRequest.Keyword,
                pagedRequest.Status,
                pagedRequest.PageIndex,
                pagedRequest.PageSize,
                pagedRequest.OrderBy
            );
            var totalCount = await _promotionService.GetTotalPromotionsCountAsync(pagedRequest.Keyword, pagedRequest.Status);

            var pagedResponse = new PagedResponse<PromotionResponse>(promotions?.Select(p =>
            {
                var response = _mapper.Map<PromotionResponse>(p);
                response.Status = Util.TranslateGeneralStatus(response.Status);
                return response;
            }) ?? Enumerable.Empty<PromotionResponse>(), totalCount, pagedRequest.PageIndex, pagedRequest.PageSize);

            return Ok(new ResponseObject<PagedResponse<PromotionResponse>>("Fetch Promotions Success", pagedResponse));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetPromotionByIdAsync(int id)
        {
            try
            {
                var promotion = await _promotionService.GetPromotionByIdAsync(id);
                if (promotion == null)
                {
                    return NotFound(new ResponseObject<PromotionResponse>($"Khuyến mãi với ID {id} không tìm thấy.", null));
                }
                var response = _mapper.Map<PromotionResponse>(promotion);
                response.Status = Util.TranslateGeneralStatus(promotion.Status);
                return Ok(new ResponseObject<PromotionResponse>("Fetch Promotion Success", response));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"An unexpected error occurred: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreatePromotionAsync([FromBody] PromotionCreateRequest request)
        {
            try
            {
                var promotion = _mapper.Map<Promotion>(request);
                await _promotionService.AddPromotionAsync(promotion);

                var promotionResponse = _mapper.Map<PromotionResponse>(promotion);
                return Ok(new ResponseObject<PromotionResponse>("Create Promotion Success", promotionResponse));
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new ResponseObject<string>($"Error: {ex.Message}"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseObject<string>($"Validation Error: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"An unexpected error occurred: {ex.Message}"));
            }
        }

        [HttpPut("{existingPromotionId}")]
        public async Task<IActionResult> UpdatePromotionAsync(int existingPromotionId, [FromBody] PromotionUpdateRequest request)
        {
            try
            {
                var promotion = _mapper.Map<Promotion>(request);
                await _promotionService.UpdatePromotionAsync(promotion, existingPromotionId);
                var updatedPromotion = await _promotionService.GetPromotionByIdAsync(existingPromotionId);
                return Ok(new ResponseObject<PromotionResponse>("Update Promotion Success",
                    _mapper.Map<PromotionResponse>(updatedPromotion)));
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
        public async Task<IActionResult> DeletePromotionAsync(int id)
        {
            try
            {
                await _promotionService.DeletePromotionAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return NotFound(new ResponseObject<string>(ex.Message));
            }
        }
    }

}
