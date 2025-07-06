using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Repository.Interfaces;

namespace TP4SCS.Repository.Implements
{
    public class ProcessRepository : GenericRepository<ServiceProcess>, IProcessRepository
    {
        public ProcessRepository(Tp4scsDevDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task AddProcessAsync(ServiceProcess process)
        {
            await InsertAsync(process);
        }
        public async Task DeleteProcessAsync(int id)
        {
            await DeleteAsync(id);
        }
        public async Task UpdateProcessAsync(ServiceProcess process)
        {
            await UpdateAsync(process);
        }
        public async Task<IEnumerable<ServiceProcess>?> GetProcessesAsync(
            string? keyword = null,
            OrderByEnum orderBy = OrderByEnum.IdDesc)
        {
            Expression<Func<ServiceProcess, bool>> filter = s =>
                (string.IsNullOrEmpty(keyword) || s.Process.Contains(keyword));

            // Bắt đầu truy vấn với bộ lọc
            var query = _dbSet.Where(filter);

            // Áp dụng sắp xếp
            query = orderBy switch
            {
                OrderByEnum.IdDesc => query.OrderByDescending(c => c.Id),
                _ => query.OrderBy(c => c.Id) // Mặc định sắp xếp theo Id tăng dần
            };
            return await query.ToListAsync();
        }
        public async Task<ServiceProcess?> GetProocessByIdAsync(int id)
        {
            return await GetByIDAsync(id);
        }

        public async Task<IEnumerable<ServiceProcess>?> GetProocessByServiceIdAsync(int id)
        {
            return await _dbContext.ServiceProcesses.Where(p => p.ServiceId == id).ToListAsync();
        }
    }
}
