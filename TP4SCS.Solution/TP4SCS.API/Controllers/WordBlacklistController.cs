using Microsoft.AspNetCore.Mvc;
using TP4SCS.Library.Models.Request.Feedback;
using TP4SCS.Library.Models.Request.WordBlacklist;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [Route("api/word-blacklist")]
    [ApiController]
    public class WordBlacklistController : ControllerBase
    {
        private readonly IWordBlacklistService _wordBlacklistService;

        public WordBlacklistController(IWordBlacklistService wordBlacklistService)
        {
            _wordBlacklistService = wordBlacklistService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBlacklistAsync([FromQuery] GetWordBlacklistRequest getWordBlacklistRequest)
        {
            var result = await _wordBlacklistService.GetBlacklistAsync(getWordBlacklistRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }

        [HttpGet("export-blacklist")]
        public async Task<IActionResult> ExportBlacklistAsync()
        {
            var result = await _wordBlacklistService.ExportAsExcelAsync();

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            var fileContent = result.Data;
            var fileName = "WordBlacklist.xlsx";

            return File(fileContent!, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost("import-blacklist")]
        public async Task<IActionResult> ImportWordBlacklist(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new
                {
                    Status = "error",
                    StatusCode = 400,
                    Message = "Không Tìm Thấy File Tải Lên Hoặc File Trống!"
                });
            }

            if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new
                {
                    Status = "error",
                    StatusCode = 400,
                    Message = "Không Đúng Định Dạng File .xlsx!"
                });
            }

            if (file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" &&
                file.ContentType != "application/vnd.ms-excel")
            {
                return BadRequest(new
                {
                    Status = "error",
                    StatusCode = 400,
                    Message = "Không Đúng Định Dạng File .xlsx!"
                });
            }

            try
            {
                using var stream = file.OpenReadStream();

                var result = await _wordBlacklistService.ImportWordBlacklistAsync(stream);

                return StatusCode(result.StatusCode, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Có Lỗi Xảy Ra!", Details = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBlacklistAsync([FromBody] UpdateWordBlacklistRequest wordBlacklistRequest)
        {
            var result = await _wordBlacklistService.AddWordAsync(wordBlacklistRequest);

            return StatusCode(result.StatusCode, result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteBlacklistAsync([FromBody] DeleteWordBlacklistRequest wordBlacklistRequest)
        {
            var result = await _wordBlacklistService.DeleteWordAsync(wordBlacklistRequest);

            if (result.StatusCode != 200)
            {
                return StatusCode(result.StatusCode, result);
            }

            return Ok(result);
        }
    }
}
