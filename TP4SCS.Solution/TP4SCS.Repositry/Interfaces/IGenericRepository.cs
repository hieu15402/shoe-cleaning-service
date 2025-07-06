using System.Linq.Expressions;

namespace TP4SCS.Repository.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>?> GetAsync(
           Expression<Func<T, bool>>? filter = null,
           Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
           string includeProperties = "",
           int? pageIndex = null,
           int? pageSize = null);

        Task<T?> GetByIDAsync(object id);

        Task InsertAsync(T entity);

        Task BulkInsertAsync(IEnumerable<T> entities);

        Task DeleteAsync(object id);

        Task BulkDeleteAsync(List<int> ids);

        Task DeleteAsync(T entityToDelete);

        Task UpdateAsync(T entityToUpdate);

        Task SaveAsync();

        Task RunInTransactionAsync(Func<Task> operations);
    }
}

