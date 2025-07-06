using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Repository.Interfaces;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class ProcessService : IProcessService
    {
        private readonly IProcessRepository _processRepository;

        public ProcessService(IProcessRepository processRepository)
        {
            _processRepository = processRepository;
        }

        public async Task AddProcessAsync(ServiceProcess process)
        {
            if (process == null)
                throw new ArgumentNullException(nameof(process));
            if (string.IsNullOrWhiteSpace(process.Process))
                throw new ArgumentException("Process cannot be null or empty.", nameof(process.Process));

            if (process.ProcessOrder <= 0)
                throw new ArgumentException("ProcessOrder must be greater than 0.", nameof(process.ProcessOrder));
            await _processRepository.AddProcessAsync(process);
        }

        public async Task DeleteProcessAsync(int id)
        {
            var processs = await _processRepository.GetProocessByIdAsync(id);
            if (processs == null)
                throw new ArgumentException($"Process with id: {id} not found.");
            await _processRepository.DeleteProcessAsync(id);
        }

        public async Task UpdateProcessAsync(ServiceProcess process, int existiedProcessId)
        {
            var existiedProcess = await _processRepository.GetProocessByIdAsync(existiedProcessId);
            if (existiedProcess == null)
                throw new ArgumentException($"Process with id: {existiedProcessId} not found.");
            if (string.IsNullOrWhiteSpace(process.Process))
                throw new ArgumentException("Process cannot be null or empty.", nameof(process.Process));

            if (process.ProcessOrder <= 0)
                throw new ArgumentException("ProcessOrder must be greater than 0.", nameof(process.ProcessOrder));
            existiedProcess.Process = process.Process;
            existiedProcess.ProcessOrder = process.ProcessOrder;

            await _processRepository.UpdateProcessAsync(existiedProcess);
        }

        public async Task<(IEnumerable<ServiceProcess>? Processes, int TotalCount)> GetProcessesAsync(
            int pageIndex = 1,
            int pageSize = 10,
            string? keyword = null,
            OrderByEnum orderBy = OrderByEnum.IdDesc)
        {
            if (pageIndex <= 0)
                throw new ArgumentException("Page index must be greater than 0.", nameof(pageIndex));

            if (pageSize <= 0)
                throw new ArgumentException("Page size must be greater than 0.", nameof(pageSize));

            // Lấy dữ liệu từ repository
            var allProcesses = await _processRepository.GetProcessesAsync(keyword, orderBy);

            // Nếu không có dữ liệu, trả về kết quả rỗng
            if (allProcesses == null || !allProcesses.Any())
            {
                return (null, 0);
            }

            // Tổng số lượng mục
            int totalCount = allProcesses.Count();

            // Áp dụng phân trang
            var pagedProcesses = allProcesses
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);

            return (pagedProcesses, totalCount);
        }


        public async Task<ServiceProcess?> GetProcessByIdAsync(int id)
        {
            var process = await _processRepository.GetProocessByIdAsync(id);
            if (process == null)
                throw new ArgumentException($"Process with id: {id} not found.");

            return process;
        }

        public async Task<(IEnumerable<ServiceProcess>? Processes, int TotalCount)> GetProcessesByServiceIdAsync(
            int serviceId,
            int pageIndex = 1,
            int pageSize = 10)
        {
            if (pageIndex <= 0)
                throw new ArgumentException("Page index must be greater than 0.", nameof(pageIndex));

            if (pageSize <= 0)
                throw new ArgumentException("Page size must be greater than 0.", nameof(pageSize));

            var allProcesses = await _processRepository.GetProocessByServiceIdAsync(serviceId);

            // Kiểm tra nếu không có dữ liệu
            if (allProcesses == null || !allProcesses.Any())
            {
                return (null, 0);
            }

            // Tổng số lượng mục
            int totalCount = allProcesses.Count();

            // Áp dụng phân trang
            var pagedProcesses = allProcesses
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);

            return (pagedProcesses, totalCount);
        }
    }
}
