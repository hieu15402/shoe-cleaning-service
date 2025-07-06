using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TP4SCS.Library.Models.Data;
using TP4SCS.Repository.Interfaces;

namespace TP4SCS.Repository.Implements
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly Tp4scsDevDatabaseContext _dbContext;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(Tp4scsDevDatabaseContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = dbContext.Set<T>();
        }

        public virtual async Task<IEnumerable<T>?> GetAsync(
           Expression<Func<T, bool>>? filter = null,
           Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
           string includeProperties = "",
           int? pageIndex = null, // Optional parameter for pagination (page number)
           int? pageSize = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            // Implementing pagination
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                // Ensure the pageIndex and pageSize are valid
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10; // Assuming a default pageSize of 10 if an invalid value is passed

                query = query.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            return await query.ToListAsync(); // Sử dụng ToListAsync để thực hiện truy vấn không đồng bộ
        }

        public virtual async Task<T?> GetByIDAsync(object id)
        {
            return await _dbSet.FindAsync(id); // Sử dụng FindAsync để tìm kiếm không đồng bộ
        }

        public virtual async Task InsertAsync(T entity)
        {
            _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;

            await _dbSet.AddAsync(entity); // Sử dụng AddAsync để thêm không đồng bộ

            await _dbContext.SaveChangesAsync(); // Lưu thay đổi không đồng bộ

            _dbContext.ChangeTracker.AutoDetectChangesEnabled = false;
        }

        public virtual async Task DeleteAsync(object id)
        {
            T? entityToDelete = await _dbSet.FindAsync(id);

            if (entityToDelete == null)
            {
                throw new KeyNotFoundException($"Entity with id {id} not found.");
            }
            await DeleteAsync(entityToDelete);
        }

        public virtual async Task DeleteAsync(T entityToDelete)
        {
            if (_dbContext.Entry(entityToDelete).State == EntityState.Detached)
            {
                _dbSet.Attach(entityToDelete);
            }
            _dbSet.Remove(entityToDelete);
            await _dbContext.SaveChangesAsync(); // Lưu thay đổi không đồng bộ
        }

        public virtual async Task UpdateAsync(T entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _dbContext.Entry(entityToUpdate).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(); // Lưu thay đổi không đồng bộ
        }

        public async Task RunInTransactionAsync(Func<Task> operations)
        {
            using (var transaction = await _dbContext.Database.BeginTransactionAsync())
            {
                try
                {
                    await operations();
                    await transaction.CommitAsync();
                }
                catch
                {
                    await transaction.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task SaveAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public async Task BulkInsertAsync(IEnumerable<T> entities)
        {
            await _dbContext.BulkInsertAsync(entities);
        }

        public async Task BulkDeleteAsync(List<int> ids)
        {
            if (ids == null || !ids.Any())
                throw new ArgumentException("The IDs collection is null or empty.");

            var entitiesToDelete = await _dbContext.Set<T>()
                .Where(e => ids.Contains(EF.Property<int>(e, "Id")))
                .ToListAsync();

            await _dbContext.BulkDeleteAsync(entitiesToDelete);
        }
    }
}
