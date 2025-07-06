using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Transaction;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.Transaction;
using TP4SCS.Repository.Interfaces;

namespace TP4SCS.Library.Repositories
{
    public interface ITransactionRepository : IGenericRepository<Transaction>
    {
        Task<(IEnumerable<TransactionResponse>?, Pagination)> GetTransactionsAsync(GetTransactionRequest getTransactionRequest);

        Task<TransactionResponse?> GetTransactionByIdAsync(int id);

        Task<Transaction?> GetTransactionByIdNoTrackingAsync(int id);

        Task CreateTransactionAsync(Transaction transaction);

        Task UpdateTransactionAsync(Transaction transaction);

        Task DeleteTransactionAsync(int id);

        Task<Dictionary<int, decimal>> SumMonthProfitAsync();

        Task<Dictionary<int, decimal>> SumYearProfitAsync();
    }
}