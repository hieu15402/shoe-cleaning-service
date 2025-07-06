using TP4SCS.Library.Models.Request.Transaction;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.Transaction;

namespace TP4SCS.Library.Services
{
    public interface ITransactionService
    {
        Task<ApiResponse<IEnumerable<TransactionResponse>?>> GetTransactionsAsync(GetTransactionRequest getTransactionRequest);

        Task<ApiResponse<TransactionResponse?>> GetTransactionByIdAsync(int id);

        Task<ApiResponse<TransactionResponse>> CreateTransactionAsync(CreateTransactionRequest createTransactionRequest);

        Task<ApiResponse<TransactionResponse>> UpdateTransactionAsync(int id, UpdateTransactionRequest updateTransactionRequest);

        Task<ApiResponse<TransactionResponse>> DeteleTransactionAsync(int id);
    }
}