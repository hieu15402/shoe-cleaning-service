using Microsoft.EntityFrameworkCore;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Transaction;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.Transaction;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Repository.Implements;

namespace TP4SCS.Library.Repositories
{
    public class TransactionRepository : GenericRepository<Transaction>, ITransactionRepository
    {
        public TransactionRepository(Tp4scsDevDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task CreateTransactionAsync(Transaction transaction)
        {
            await InsertAsync(transaction);
        }

        public async Task DeleteTransactionAsync(int id)
        {
            await DeleteAsync(id);
        }

        public async Task<TransactionResponse?> GetTransactionByIdAsync(int id)
        {
            return await _dbContext.Transactions
                .Where(t => t.Id == id)
                .Join(
                    _dbContext.Accounts.AsNoTracking(),
                    transaction => transaction.AccountId,
                    account => account.Id,
                    (transaction, account) => new { transaction, account }
                )
                .Select(t => new TransactionResponse
                {
                    Id = t.transaction.Id,
                    AccountId = t.transaction.AccountId,
                    AccountName = t.account.FullName,
                    PackName = t.transaction.PackName,
                    Balance = t.transaction.Balance,
                    ProcessTime = t.transaction.ProcessTime,
                    Description = t.transaction.Description,
                    PaymentMethod = t.transaction.PaymentMethod,
                    Status = t.transaction.Status
                })
                .SingleOrDefaultAsync();

        }

        public async Task<Transaction?> GetTransactionByIdNoTrackingAsync(int id)
        {
            return await _dbContext.Transactions.AsNoTracking().SingleOrDefaultAsync(t => t.Id == id);
        }

        public async Task<(IEnumerable<TransactionResponse>?, Pagination)> GetTransactionsAsync(GetTransactionRequest getTransactionRequest)
        {
            var transactionsQuery = _dbContext.Transactions
                .AsNoTracking()
                .Join(
                    _dbContext.Accounts.AsNoTracking(),
                    transaction => transaction.AccountId,
                    account => account.Id,
                    (transaction, account) => new TransactionResponse
                    {
                        Id = transaction.Id,
                        AccountId = transaction.AccountId,
                        AccountName = account.FullName,
                        PackName = transaction.PackName,
                        Balance = transaction.Balance,
                        ProcessTime = transaction.ProcessTime,
                        Description = transaction.Description,
                        PaymentMethod = transaction.PaymentMethod,
                        Status = transaction.Status
                    }
                );

            if (getTransactionRequest.AccountId.HasValue)
            {
                transactionsQuery = transactionsQuery
                    .Where(t => t.AccountId == getTransactionRequest.AccountId)
                    .OrderByDescending(t => t.ProcessTime);
            }

            var transactions = transactionsQuery
                .OrderByDescending(t => t.ProcessTime)
                .AsQueryable();

            //Search
            if (!string.IsNullOrEmpty(getTransactionRequest.SearchKey))
            {
                string searchKey = getTransactionRequest.SearchKey;
                transactions = transactions.Where(t => EF.Functions.Like(t.AccountName, $"%{searchKey}%") ||
                    EF.Functions.Like(t.PackName, $"%{searchKey}%") ||
                    EF.Functions.Like(t.PaymentMethod, $"%{searchKey}%"));
            }

            //Status Sort
            if (getTransactionRequest.Status.HasValue)
            {
                transactions = getTransactionRequest.Status switch
                {
                    TransactionStatus.PENDING => transactions.Where(t => t.Status.Equals(StatusConstants.PENDING)),
                    TransactionStatus.PROCESSING => transactions.Where(t => t.Status.Equals(StatusConstants.PROCESSING)),
                    TransactionStatus.COMPLETED => transactions.Where(t => t.Status.Equals(StatusConstants.COMPLETED)),
                    TransactionStatus.EXPIRED => transactions.Where(t => t.Status.Equals(StatusConstants.EXPIRED)),
                    TransactionStatus.FAILED => transactions.Where(t => t.Status.Equals(StatusConstants.FAILED)),
                    _ => transactions
                };
            }

            //Order Sort
            if (getTransactionRequest.SortBy.HasValue)
            {
                transactions = getTransactionRequest.SortBy switch
                {
                    TransactionSortOption.ACCOUNTNAME => getTransactionRequest.IsDecsending
                                ? transactions.OrderByDescending(t => t.AccountName)
                                : transactions.OrderBy(t => t.AccountName),
                    TransactionSortOption.PACKNAME => getTransactionRequest.IsDecsending
                                  ? transactions.OrderByDescending(t => t.PackName)
                                  : transactions.OrderBy(t => t.PackName),
                    TransactionSortOption.BALANCE => getTransactionRequest.IsDecsending
                                  ? transactions.OrderByDescending(t => t.Balance)
                                  : transactions.OrderBy(t => t.Balance),
                    TransactionSortOption.TIME => getTransactionRequest.IsDecsending
                                  ? transactions.OrderByDescending(t => t.ProcessTime)
                                  : transactions.OrderBy(t => t.ProcessTime),
                    TransactionSortOption.PAYMENT => getTransactionRequest.IsDecsending
                                  ? transactions.OrderByDescending(t => t.PaymentMethod)
                                  : transactions.OrderBy(t => t.PaymentMethod),
                    _ => transactions
                };
            }

            //Count Total Data
            int totalData = await transactions.AsNoTracking().CountAsync();

            //Paging
            int skipNum = (getTransactionRequest.PageNum - 1) * getTransactionRequest.PageSize;
            transactions = transactions.Skip(skipNum).Take(getTransactionRequest.PageSize);

            var result = await transactions.ToListAsync();

            int totalPage = (int)Math.Ceiling((decimal)totalData / getTransactionRequest.PageSize);

            var pagination = new Pagination(totalData, getTransactionRequest.PageSize, getTransactionRequest.PageNum, totalPage);

            return (result, pagination);
        }

        public async Task<Dictionary<int, decimal>> SumMonthProfitAsync()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var transactions = await _dbContext.Transactions
                .AsNoTracking()
                .Where(t =>
                    t.ProcessTime.Month == currentMonth &&
                    t.ProcessTime.Year == currentYear)
                .ToListAsync();

            var dailyProfits = transactions
                .GroupBy(t => t.ProcessTime.Day)
                .ToDictionary(
                    group => group.Key,
                    group => group.Sum(t => t.Balance)
                );

            var result = Enumerable.Range(1, 31)
                .ToDictionary(
                    day => day,
                    day => dailyProfits.ContainsKey(day) ? dailyProfits[day] : 0m
                );

            return result;
        }

        public async Task<Dictionary<int, decimal>> SumYearProfitAsync()
        {
            var currentYear = DateTime.Now.Year;

            var transactions = await _dbContext.Transactions
                .AsNoTracking()
                .Where(t =>
                    t.ProcessTime.Year == currentYear)
                .ToListAsync();

            var monthlyProfits = transactions
                .GroupBy(t => t.ProcessTime.Month)
                .ToDictionary(
                    group => group.Key,
                    group => group.Sum(t => t.Balance)
                );

            var result = Enumerable.Range(1, 12)
                .ToDictionary(
                    month => month,
                    month => monthlyProfits.ContainsKey(month) ? monthlyProfits[month] : 0m
                );

            return result;
        }

        public async Task UpdateTransactionAsync(Transaction transaction)
        {
            await UpdateAsync(transaction);
        }
    }
}
