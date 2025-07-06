using TP4SCS.Library.Models.Request.Account;
using TP4SCS.Library.Models.Request.BusinessProfile;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Library.Models.Response.Account;
using TP4SCS.Library.Models.Response.Business;
using TP4SCS.Library.Models.Response.General;

namespace TP4SCS.Services.Interfaces
{
    public interface IAccountService
    {
        Task<ApiResponse<IEnumerable<AccountResponse>?>> GetAccountsAsync(GetAccountRequest getAccountRequest);

        Task<ApiResponse<IEnumerable<EmployeeResponse>?>> GetEmployeesAsync(GetEmployeeRequest getEmployeeRequest);

        Task<ApiResponse<AccountResponse?>> GetAccountByIdAsync(int id);

        Task<ApiResponse<AccountResponse?>> GetAccountByEmailAsync(string email);

        Task<int> GetAccountMaxIdAsync();

        Task<ApiResponse<AccountResponse>> CreateEmployeeAccountAsync(int id, CreateEmployeeRequest createEmployeeRequest);

        Task<ApiResponse<AccountResponse>> CreateModeratorAccountAsync(CreateModeratorRequest createModeratorRequest);

        Task<ApiResponse<AccountResponse>> UpdateAccountAsync(int id, UpdateAccountRequest updateAccountRequest);

        Task<ApiResponse<BusinessResponseV2>> UpdateAccountToOwnerAsync(int id, CreateBusinessRequest createBusinessRequest);

        Task<ApiResponse<AccountResponse>> UpdateAccountPasswordAsync(int id, UpdateAccountPasswordRequest updateAccountPasswordRequest);

        Task<ApiResponse<AccountResponse>> UpdateAccountStatusForAdminAsync(int id, UpdateStatusRequest updateStatusRequest);

        Task<ApiResponse<AccountResponse>> DeleteAccountAsync(int id);

    }
}
