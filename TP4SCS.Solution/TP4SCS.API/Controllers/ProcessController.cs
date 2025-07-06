using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Library.Models.Request.Process;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.Process;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.API.Controllers
{
    [Route("api/processes")]
    [ApiController]
    public class ProcessController : ControllerBase
    {
        private readonly IProcessService _processService;
        private readonly IMapper _mapper;

        public ProcessController(IProcessService processService, IMapper mapper)
        {
            _processService = processService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetProcesses(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? keyword = null,
            [FromQuery] OrderByEnum orderBy = OrderByEnum.IdDesc)
        {
            var (processes, totalCount) = await _processService.GetProcessesAsync(pageIndex, pageSize, keyword, orderBy);
            if (processes == null || !processes.Any())
            {
                return NotFound(new ResponseObject<string>("Không tìm thấy quá trình nào."));
            }
            var processResponse = _mapper.Map<IEnumerable<ProcessResponse>>(processes);

            PagedResponse<ProcessResponse> pagedResponse = new PagedResponse<ProcessResponse>(
                processResponse,
                totalCount,
                pageIndex,
                pageSize
            );
            return Ok(new ResponseObject<PagedResponse<ProcessResponse>>("Lấy quá trình của dịch vụ thành công", pagedResponse));
        }
        [HttpGet("by-service/{id}")]
        public async Task<IActionResult> GetProcessesByServiceId(
            [FromRoute] int id,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var (processes, totalCount) = await _processService.GetProcessesByServiceIdAsync(id, pageIndex, pageSize);

            if (processes == null || !processes.Any())
            {
                return Ok(new ResponseObject<IEnumerable<ProcessResponse>>("Không tìm thấy quá trình nào cho dịch vụ này.", new List<ProcessResponse>()));
            }
            var processResponse = _mapper.Map<IEnumerable<ProcessResponse>>(processes);

            var pagedResponse = new PagedResponse<ProcessResponse>(
                processResponse,
                totalCount,
                pageIndex,
                pageSize
            );

            // Trả về kết quả
            return Ok(new ResponseObject<PagedResponse<ProcessResponse>>(
                "Lấy quá trình của dịch vụ thành công",
                pagedResponse
            ));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProcessById([FromRoute] int id)
        {
            try
            {
                var process = await _processService.GetProcessByIdAsync(id);

                var processResponse = _mapper.Map<ProcessResponse>(process);

                return Ok(new ResponseObject<ProcessResponse>("Lấy quá trình thành công", processResponse));
            }
            catch (ArgumentException ex)
            {
                return NotFound(new ResponseObject<string>(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ResponseObject<string>(ex.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddProcess([FromBody] ProcessCreateRequest request)
        {
            if (request == null)
                return BadRequest(new ResponseObject<string>("Dữ liệu không hợp lệ."));

            try
            {
                // Ánh xạ từ ProcessCreateRequest sang ServiceProcess
                var process = _mapper.Map<ServiceProcess>(request);

                // Gọi service để thêm quá trình mới
                await _processService.AddProcessAsync(process);

                // Trả về kết quả thành công
                return Ok(new ResponseObject<string>("Thêm quá trình thành công."));
            }
            catch (ArgumentException ex)
            {
                // Trả về lỗi nếu có vấn đề về dữ liệu
                return BadRequest(new ResponseObject<string>(ex.Message));
            }
            catch (Exception ex)
            {
                // Trả về lỗi server nếu có lỗi không mong muốn
                return StatusCode(500, new ResponseObject<string>(ex.Message));
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProcess(int id, [FromBody] ProcessUpdateRequest request)
        {
            if (request == null)
                return BadRequest(new ResponseObject<string>("Dữ liệu không hợp lệ.", null));

            try
            {
                var processToUpdate = _mapper.Map<ServiceProcess>(request);

                await _processService.UpdateProcessAsync(processToUpdate, id);
                return Ok(new ResponseObject<string>("Cập nhật quá trình thành công.", null));
            }
            catch (ArgumentException ex)
            {
                // Trả về lỗi nếu không tìm thấy quá trình
                return NotFound(new ResponseObject<string>(ex.Message, null));
            }
            catch (Exception ex)
            {
                // Trả về lỗi server nếu có lỗi không mong muốn
                return StatusCode(500, new ResponseObject<string>(ex.Message, null));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProcess(int id)
        {
            try
            {
                // Gọi service để xóa quá trình
                await _processService.DeleteProcessAsync(id);

                // Trả về kết quả thành công
                return Ok(new ResponseObject<string>("Xóa quá trình thành công.", null));
            }
            catch (ArgumentException ex)
            {
                // Nếu không tìm thấy quá trình, trả về lỗi 404
                return NotFound(new ResponseObject<string>(ex.Message, null));
            }
            catch (Exception ex)
            {
                // Trả về lỗi server nếu có lỗi không mong muốn
                return StatusCode(500, new ResponseObject<string>(ex.Message, null));
            }
        }


    }
}
