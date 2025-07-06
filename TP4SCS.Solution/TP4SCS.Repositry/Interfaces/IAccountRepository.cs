using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Account;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Library.Models.Response.Account;
using TP4SCS.Library.Models.Response.General;

namespace TP4SCS.Repository.Interfaces
{
    public interface IAccountRepository : IGenericRepository<Account>
    {
        Task<(IEnumerable<Account>?, Pagination)> GetAccountsAsync(GetAccountRequest getAccountRequest);

        Task<Account?> GetAccountByIdAsync(int id);

        Task<Account?> GetAccountByIdNoTrackingAsync(int id);

        Task<string> GetAccountEmailByIdAsync(int id);

        Task<Account?> GetAccountEmailByTicketIdAsync(int id);

        Task<List<string>?> GetBranchEmailsByOrderIdAsync(int id);

        Task<(IEnumerable<EmployeeResponse>?, Pagination)> GetEmployeesAsync(GetEmployeeRequest getEmployeeRequest);

        Task<Account?> GetAccountByIdForAdminAsync(int id);

        Task<Account?> GetAccountByEmailAsync(string email);

        Task<Account?> GetAccountByEmailNoTrackingAsync(string email);

        Task<bool> IsEmailExistedAsync(string email);

        Task<bool> IsPhoneExistedAsync(string phone);

        Task<int> GetAccountMaxIdAsync();

        Task<int> CountAccountDataAsync();

        Task CreateAccountAsync(Account account);

        Task UpdateAccountAsync(Account account);

        Task DeleteAccountAsync(int id);
    }
}
