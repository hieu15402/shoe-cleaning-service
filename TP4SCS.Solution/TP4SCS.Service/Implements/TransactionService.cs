using MapsterMapper;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Transaction;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.Transaction;
using TP4SCS.Library.Repositories;
using TP4SCS.Repository.Interfaces;

namespace TP4SCS.Library.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly ITransactionRepository _transactionRepository;
        private readonly IPlatformPackRepository _platformPackRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IMapper _mapper;

        public TransactionService(ITransactionRepository transactionRepository,
            IPlatformPackRepository subscriptionPackRepository,
            IAccountRepository accountRepository,
            IMapper mapper)
        {
            _transactionRepository = transactionRepository;
            _platformPackRepository = subscriptionPackRepository;
            _accountRepository = accountRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<TransactionResponse>> CreateTransactionAsync(CreateTransactionRequest createTransactionRequest)
        {
            var account = await _accountRepository.GetAccountByIdNoTrackingAsync(createTransactionRequest.AccountId);

            if (account == null)
            {
                return new ApiResponse<TransactionResponse>("error", 404, "Không Tìm Thấy Thông Tin Tài Khoản!");
            }

            var pack = await _platformPackRepository.GetPackByIdNoTrackingAsync(createTransactionRequest.PackId);

            if (pack == null)
            {
                return new ApiResponse<TransactionResponse>("error", 404, "Không Tìm Thấy Thông Tin Gói Đăng Kí!");
            }

            var newTransaction = _mapper.Map<Transaction>(createTransactionRequest);
            newTransaction.PackName = pack.Name;
            newTransaction.Description = $"Thanh Toán {pack.Name} Bằng {createTransactionRequest.PaymentMethod}";

            try
            {
                await _transactionRepository.CreateTransactionAsync(newTransaction);

                var newTran = await GetTransactionByIdAsync(newTransaction.Id);

                return new ApiResponse<TransactionResponse>("success", "Tạo Giao Dịch Thành Công!", newTran.Data, 201);
            }
            catch (Exception)
            {
                return new ApiResponse<TransactionResponse>("error", 400, "Tạo Giao Dịch Thất Bại!");
            }
        }

        public async Task<ApiResponse<TransactionResponse>> DeteleTransactionAsync(int id)
        {
            var transaction = await _transactionRepository.GetTransactionByIdNoTrackingAsync(id);

            if (transaction == null)
            {
                return new ApiResponse<TransactionResponse>("error", 404, "Không Tìm Thấy Thông Tin Giao Dịch!");
            }

            try
            {
                await _transactionRepository.DeleteAsync(transaction);

                return new ApiResponse<TransactionResponse>("success", "Xoá Thông Tin Giao Dịch Thành Công!", null);
            }
            catch (Exception)
            {
                return new ApiResponse<TransactionResponse>("error", 404, "Xoá Thông Tin Giao Dịch Thất Bại!");
            }
        }

        public async Task<ApiResponse<TransactionResponse?>> GetTransactionByIdAsync(int id)
        {
            var transaction = await _transactionRepository.GetTransactionByIdAsync(id);

            if (transaction == null)
            {
                return new ApiResponse<TransactionResponse?>("error", 404, "Không Tìm Thấy Thông Tin Giao Dịch!");
            }

            return new ApiResponse<TransactionResponse?>("success", "Lấy Thông Tin Giao Dịch Thành Công", transaction);
        }

        public async Task<ApiResponse<IEnumerable<TransactionResponse>?>> GetTransactionsAsync(GetTransactionRequest getTransactionRequest)
        {
            var (transactions, pagination) = await _transactionRepository.GetTransactionsAsync(getTransactionRequest);

            if (transactions == null)
            {
                return new ApiResponse<IEnumerable<TransactionResponse>?>("error", 404, "Thông Tin Giao Dịch Trống!");
            }

            return new ApiResponse<IEnumerable<TransactionResponse>?>("success", "Lấy Thông Tin Giao Dịch Thành Công", transactions, 200, pagination);
        }

        public async Task<ApiResponse<TransactionResponse>> UpdateTransactionAsync(int id, UpdateTransactionRequest updateTransactionRequest)
        {
            var pack = await _platformPackRepository.GetPackByIdNoTrackingAsync(updateTransactionRequest.PackId);

            if (pack == null)
            {
                return new ApiResponse<TransactionResponse>("error", 404, "Không Tìm Thấy Thông Tin Gói Đăng Kí!");
            }

            var oldTransaction = await _transactionRepository.GetTransactionByIdAsync(id);

            if (oldTransaction == null)
            {
                return new ApiResponse<TransactionResponse>("error", 404, "Không Tìm Thấy Thông Tin Giao Dịch!");
            }

            var newData = _mapper.Map(updateTransactionRequest, oldTransaction);

            var newTransaction = _mapper.Map<Transaction>(newData);
            newTransaction.PackName = pack.Name;
            newTransaction.Description = $"Thanh Toán {pack.Name} Bằng {updateTransactionRequest.PaymentMethod}";

            try
            {
                await _transactionRepository.UpdateTransactionAsync(newTransaction);

                return new ApiResponse<TransactionResponse>("error", "Cập Nhập Thông Tin Giao Dịch Thành Công!", null);
            }
            catch (Exception)
            {
                return new ApiResponse<TransactionResponse>("error", 400, "Cập Nhập Thông Tin Giao Dịch Thất Bại!");
            }
        }
    }
}