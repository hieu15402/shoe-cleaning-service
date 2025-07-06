using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;

namespace TP4SCS.Repository.Interfaces
{
    public interface IProcessRepository
    {
        Task AddProcessAsync(ServiceProcess process);
        Task DeleteProcessAsync(int id);
        Task UpdateProcessAsync(ServiceProcess process);
        Task<IEnumerable<ServiceProcess>?> GetProcessesAsync(
            string? keyword = null,
            OrderByEnum orderBy = OrderByEnum.IdDesc);
        Task<ServiceProcess?> GetProocessByIdAsync(int id);
        Task<IEnumerable<ServiceProcess>?> GetProocessByServiceIdAsync(int id);
    }
}
