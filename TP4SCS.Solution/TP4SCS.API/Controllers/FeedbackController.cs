using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Feedback;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Library.Models.Response.Feedback;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [Route("api/feedbacks")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IFeedbackService _feedbackService;
        private readonly IMapper _mapper;

        public FeedbackController(IHttpClientFactory httpClientFactory,
            IFeedbackService feedbackService,
            IMapper mapper)
        {
            _httpClientFactory = httpClientFactory;
            _feedbackService = feedbackService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetFeedbacks(string? status,
            OrderByEnum orderBy = OrderByEnum.IdDesc,
            int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var feedbacks = await _feedbackService.GetFeedbacks(status, orderBy);

                var response = _mapper.Map<IEnumerable<FeedbackResponseForAdmin>>(feedbacks);

                var totalCount = response.Count();
                var pagedFeedbacks = response
                    .Skip((pageIndex - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var pagedResponse = new PagedResponse<FeedbackResponseForAdmin>(pagedFeedbacks, totalCount, pageIndex, pageSize);

                return Ok(new ResponseObject<PagedResponse<FeedbackResponseForAdmin>>("Lấy danh sách đánh giá thành công", pagedResponse));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Đã xảy ra lỗi không mong muốn: {ex.Message}"));
            }
        }

        // GET: api/feedbacks/{serviceId}
        [HttpGet("services/{id}")]
        public async Task<IActionResult> GetFeedbacksByServiceId(int id)
        {
            try
            {
                var feedbacks = await _feedbackService.GetFeedbackByServiceId(id);
                var response = _mapper.Map<IEnumerable<FeedbackResponse>>(feedbacks);
                return Ok(new ResponseObject<IEnumerable<FeedbackResponse>>("Lấy danh sách đánh giá thành công", response));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Đã xảy ra lỗi không mong muốn: {ex.Message}"));
            }
        }

        [HttpGet("branches/{id}")]
        public async Task<IActionResult> GetFeedbacksByBranchId(int id, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var (feedbacks, totalCount) = await _feedbackService.GetFeedbackByBranchIdIdAsync(id, pageIndex, pageSize);

                // Ánh xạ danh sách Feedbacks sang FeedbackResponse
                var response = _mapper.Map<IEnumerable<FeedbackResponse>>(feedbacks);

                // Tạo đối tượng phân trang
                var pagedResponse = new PagedResponse<FeedbackResponse>(response, totalCount, pageIndex, pageSize);

                return Ok(new ResponseObject<PagedResponse<FeedbackResponse>>("Lấy danh sách đánh giá thành công", pagedResponse));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Đã xảy ra lỗi không mong muốn: {ex.Message}"));
            }
        }

        [HttpGet("business/{id}")]
        public async Task<IActionResult> GetFeedbacksByBusinesshId(int id, int pageIndex = 1, int pageSize = 10)
        {
            try
            {
                var (feedbacks, totalCount) = await _feedbackService.GetFeedbackByBusinessIdIdAsync(id, pageIndex, pageSize);

                // Ánh xạ danh sách Feedbacks sang FeedbackResponse
                var response = _mapper.Map<IEnumerable<FeedbackResponse>>(feedbacks);

                // Tạo đối tượng phân trang
                var pagedResponse = new PagedResponse<FeedbackResponse>(response, totalCount, pageIndex, pageSize);

                return Ok(new ResponseObject<PagedResponse<FeedbackResponse>>("Lấy danh sách đánh giá thành công", pagedResponse));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Đã xảy ra lỗi không mong muốn: {ex.Message}"));
            }
        }

        [HttpGet("accounts/{id}")]
        public async Task<IActionResult> GetFeedbacksByAccountId(int id)
        {
            try
            {
                var feedbacks = await _feedbackService.GetFeedbackByAccountId(id);
                var response = _mapper.Map<IEnumerable<FeedbackResponse>>(feedbacks);
                return Ok(new ResponseObject<IEnumerable<FeedbackResponse>>("Lấy danh sách đánh giá thành công", response));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Đã xảy ra lỗi không mong muốn: {ex.Message}"));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetFeedbackById(int id)
        {
            try
            {
                var feedback = await _feedbackService.GetFeedbackByIdAsync(id);
                var response = _mapper.Map<FeedbackResponse>(feedback);
                return Ok(new ResponseObject<FeedbackResponse>("Lấy danh sách đánh giá thành công", response));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Đã xảy ra lỗi không mong muốn: {ex.Message}"));
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFeedback([FromBody] FeedbackRequest feedbackRequest)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("ChatGPT");
                var feedback = _mapper.Map<Feedback>(feedbackRequest);
                await _feedbackService.AddFeedbacksAsync(httpClient, feedback);
                return Ok(new ResponseObject<string>("Tạo đánh giá thành công"));
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new ResponseObject<string>($"Lỗi: {ex.Message}"));
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new ResponseObject<string>($"Lỗi: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Đã xảy ra lỗi không mong muốn: {ex.Message}"));
            }
        }
        // DELETE: api/feedbacks/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFeedback(int id)
        {
            try
            {
                await _feedbackService.DeleteFeedbackAsync(id);
                return Ok(new ResponseObject<string>("Xóa đánh giá thành công"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseObject<string>($"Lỗi: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Đã xảy ra lỗi không mong muốn: {ex.Message}"));
            }
        }

        // PUT: api/feedbacks/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFeedback(int id, [FromBody] FeedbackUpdateRequest feedbackRequest)
        {
            try
            {
                await _feedbackService.UpdateFeedbackAsync(feedbackRequest.IsValidAsset, feedbackRequest.IsValidContent, feedbackRequest.Status, id);
                return Ok(new ResponseObject<string>("Cập nhật đánh giá thành công"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseObject<string>($"Lỗi: {ex.Message}"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseObject<string>($"Lỗi: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Đã xảy ra lỗi không mong muốn: {ex.Message}"));
            }
        }

        [HttpPut("{id}/content")]
        public async Task<IActionResult> UpdateFeedback(int id, [FromBody] FeedbackUpdateRequestV2 request)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("ChatGPT");
                var feedback = _mapper.Map<Feedback>(request);
                await _feedbackService.UpdateContentFeedbackAsync(feedback, id, httpClient);
                return Ok(new ResponseObject<string>("Cập nhật đánh giá thành công"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseObject<string>($"Lỗi: {ex.Message}"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseObject<string>($"Lỗi: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Đã xảy ra lỗi không mong muốn: {ex.Message}"));
            }
        }

        [HttpPut("{id}/reply")]
        public async Task<IActionResult> ReplyFeedback(int id, [FromBody] string reply)
        {
            try
            {
                await _feedbackService.ReplyFeedbackAsync(reply, id);
                return Ok(new ResponseObject<string>("Cập nhật đánh giá thành công"));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new ResponseObject<string>($"Lỗi: {ex.Message}"));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ResponseObject<string>($"Lỗi: {ex.Message}"));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>($"Đã xảy ra lỗi không mong muốn: {ex.Message}"));
            }
        }
    }
}
