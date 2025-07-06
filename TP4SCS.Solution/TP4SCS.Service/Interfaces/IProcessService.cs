using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;

namespace TP4SCS.Services.Interfaces
{
    public interface IProcessService
    {
        Task AddProcessAsync(ServiceProcess process);

        Task DeleteProcessAsync(int id);

        Task UpdateProcessAsync(ServiceProcess process, int existiedProcessId);

        Task<(IEnumerable<ServiceProcess>? Processes, int TotalCount)> GetProcessesAsync(
            int pageIndex = 1,
            int pageSize = 10,
            string? keyword = null,
            OrderByEnum orderBy = OrderByEnum.IdDesc);

        Task<ServiceProcess?> GetProcessByIdAsync(int id);

        Task<(IEnumerable<ServiceProcess>? Processes, int TotalCount)> GetProcessesByServiceIdAsync(
            int serviceId,
            int pageIndex = 1,
            int pageSize = 10);
    }
}
