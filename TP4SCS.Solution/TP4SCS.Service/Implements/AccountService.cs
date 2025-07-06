using Mapster;
using MapsterMapper;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Account;
using TP4SCS.Library.Models.Request.Branch;
using TP4SCS.Library.Models.Request.BusinessProfile;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Library.Models.Response.Account;
using TP4SCS.Library.Models.Response.Business;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Library.Utils.Utils;
using TP4SCS.Repository.Interfaces;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class AccountService : IAccountService
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IAuthService _authService;
        private readonly IBusinessBranchService _businessBranchService;
        private readonly IMapper _mapper;
        private readonly Util _util;

        public AccountService(IAccountRepository accountRepository,
            IBusinessRepository businessRepository,
            IAuthService authService,
            IBusinessBranchService businessBranchService,
            IMapper mapper,
            Util util)
        {
            _accountRepository = accountRepository;
            _authService = authService;
            _businessRepository = businessRepository;
            _businessBranchService = businessBranchService;
            _mapper = mapper;
            _util = util;
        }

        //Generate Random Password
        private string GanerateRandomPassword()
        {
            return Guid.NewGuid().ToString();
        }

        //Create Employee Account
        public async Task<ApiResponse<AccountResponse>> CreateEmployeeAccountAsync(int id, CreateEmployeeRequest createEmployeeRequest)
        {
            var isEmailExisted = await _accountRepository.IsEmailExistedAsync(createEmployeeRequest.Email.Trim().ToLowerInvariant());

            if (isEmailExisted == true)
            {
                return new ApiResponse<AccountResponse>("error", 400, "Email đã được sử dụng!");
            }

            var isPhoneExisted = await _accountRepository.IsPhoneExistedAsync(createEmployeeRequest.Phone.Trim());

            if (isPhoneExisted == true)
            {
                return new ApiResponse<AccountResponse>("error", 400, "Số điện thoại đã được sử dụng!");
            }

            var password = GanerateRandomPassword();

            var newAccount = _mapper.Map<Account>(createEmployeeRequest);
            newAccount.PasswordHash = _util.HashPassword(password);
            newAccount.FullName = _util.FormatStringName(createEmployeeRequest.FullName);
            newAccount.CreatedByOwnerId = id;

            var newBranchEmp = new UpdateBranchEmployeeRequest();
            newBranchEmp.EmployeeIds = new List<int>();
            newBranchEmp.IsDeleted = false;

            try
            {
                await _accountRepository.RunInTransactionAsync(async () =>
                {
                    await _accountRepository.InsertAsync(newAccount);

                    newBranchEmp.EmployeeIds.Add(newAccount.Id);

                    await _businessBranchService.UpdateBranchEmployeeAsync(createEmployeeRequest.BranchId, newBranchEmp);
                });

                var newAcc = await GetAccountByIdAsync(newAccount.Id);

                if (newAcc == null)
                {
                    return new ApiResponse<AccountResponse>("error", 400, "Tạo Tài Khoản Thất Bại!");
                }

                await _authService.SendAccountInfoEmail(newAccount.Email, password);

                return new ApiResponse<AccountResponse>("success", "Tạo Tài Khoản Thành Công!", newAcc.Data, 201);
            }
            catch (Exception)
            {
                return new ApiResponse<AccountResponse>("error", 400, "Tạo Tài Khoản Thất Bại!");
            }
        }

        //Create Moderator Account
        public async Task<ApiResponse<AccountResponse>> CreateModeratorAccountAsync(CreateModeratorRequest createModeratorRequest)
        {
            var isEmailExisted = await _accountRepository.IsEmailExistedAsync(createModeratorRequest.Email.Trim().ToLowerInvariant());

            if (isEmailExisted == true)
            {
                return new ApiResponse<AccountResponse>("error", 400, "Email đã được sử dụng!");
            }

            var isPhoneExisted = await _accountRepository.IsPhoneExistedAsync(createModeratorRequest.Phone.Trim());

            if (isPhoneExisted == true)
            {
                return new ApiResponse<AccountResponse>("error", 400, "Số điện thoại đã được sử dụng!");
            }

            var password = GanerateRandomPassword();

            var newAccount = _mapper.Map<Account>(createModeratorRequest);
            newAccount.PasswordHash = _util.HashPassword(password);
            newAccount.FullName = _util.FormatStringName(newAccount.FullName);

            try
            {
                await _accountRepository.InsertAsync(newAccount);

                var newAcc = await GetAccountByIdAsync(newAccount.Id);

                if (newAcc == null)
                {
                    return new ApiResponse<AccountResponse>("error", 400, "Tạo Tài Khoản Thất Bại!");
                }

                await _authService.SendAccountInfoEmail(newAccount.Email, password);

                return new ApiResponse<AccountResponse>("success", "Tạo Tài Khoản Thành Công!", newAcc.Data, 201);
            }
            catch (Exception)
            {
                return new ApiResponse<AccountResponse>("error", 400, "Tạo Tài Khoản Thất Bại!");
            }
        }


        //Delete Account
        public async Task<ApiResponse<AccountResponse>> DeleteAccountAsync(int id)
        {
            var account = await _accountRepository.GetAccountByIdAsync(id);

            if (account == null)
            {
                return new ApiResponse<AccountResponse>("error", 400, "Tài Khoản Không Tồn Tại!");
            }

            account.Status = "INACTIVE";

            try
            {
                if (account.Role.Equals(RoleConstants.EMPLOYEE))
                {
                    await _accountRepository.DeleteAccountAsync(account.Id);
                }
                else
                {
                    await _accountRepository.UpdateAccountAsync(account);
                }

                return new ApiResponse<AccountResponse>("success", "Xoá Tài Khoản Thành Công!", null);
            }
            catch (Exception)
            {
                return new ApiResponse<AccountResponse>("error", 400, "Xoá Tài Khoản Thất Bại!");
            }

        }

        //Get Account By Email
        public async Task<ApiResponse<AccountResponse?>> GetAccountByEmailAsync(string email)
        {
            var account = await _accountRepository.GetAccountByEmailAsync(email);

            if (account == null)
            {
                return new ApiResponse<AccountResponse?>("error", 400, "Tài Khoản Không Tồn Tại!");
            }

            var data = _mapper.Map<AccountResponse>(account);

            return new ApiResponse<AccountResponse?>("success", "Lấy dữ liệu thành công!", data, 200);
        }

        //Get Account By Id
        public async Task<ApiResponse<AccountResponse?>> GetAccountByIdAsync(int id)
        {
            var account = await _accountRepository.GetAccountByIdAsync(id);

            if (account == null)
            {
                return new ApiResponse<AccountResponse?>("error", 404, "Tài Khoản Không Tồn Tại!");
            }

            account.Role = _util.TranslateAccountRole(account.Role);
            account.Status = _util.TranslateAccountStatus(account.Status);

            var data = _mapper.Map<AccountResponse>(account);

            return new ApiResponse<AccountResponse?>("success", "Lấy dữ liệu thành công!", data, 200);
        }

        //Get Account Max Id
        public async Task<int> GetAccountMaxIdAsync()
        {
            return await _accountRepository.GetAccountMaxIdAsync();
        }

        //Get Accounts
        public async Task<ApiResponse<IEnumerable<AccountResponse>?>> GetAccountsAsync(GetAccountRequest getAccountRequest)
        {
            var (accounts, pagination) = await _accountRepository.GetAccountsAsync(getAccountRequest);

            if (accounts == null)
            {
                return new ApiResponse<IEnumerable<AccountResponse>?>("error", 404, "Tài Khoản Trống!");
            }

            var data = accounts.Adapt<IEnumerable<AccountResponse>>();

            return new ApiResponse<IEnumerable<AccountResponse>?>("success", "Lấy dữ liệu thành công!", data, 200, pagination);
        }

        //Update Account
        public async Task<ApiResponse<AccountResponse>> UpdateAccountAsync(int id, UpdateAccountRequest updateAccountRequest)
        {
            var oldAccount = await _accountRepository.GetAccountByIdAsync(id);

            if (oldAccount == null)
            {
                return new ApiResponse<AccountResponse>("error", 404, "Tài Khoản Không Tồn Tại!");
            }

            var newAccount = _mapper.Map(updateAccountRequest, oldAccount);
            newAccount.FullName = _util.FormatStringName(updateAccountRequest.FullName);

            try
            {
                await _accountRepository.UpdateAccountAsync(newAccount);

                return new ApiResponse<AccountResponse>("success", "Cập Nhập Tài Khoản Thành Công!", null);
            }
            catch (Exception)
            {
                return new ApiResponse<AccountResponse>("error", 400, "Cập Nhập Tài Khoản Thất Bại!");
            }
        }

        //Update Account Password
        public async Task<ApiResponse<AccountResponse>> UpdateAccountPasswordAsync(int id, UpdateAccountPasswordRequest updateAccountPasswordRequest)
        {
            var oldAccount = await _accountRepository.GetAccountByIdAsync(id);

            if (oldAccount == null)
            {
                return new ApiResponse<AccountResponse>("error", 404, "Tài Khoản Không Tồn Tại!");
            }

            if (!_util.CompareHashedPassword(updateAccountPasswordRequest.OldPassword, oldAccount.PasswordHash))
            {
                return new ApiResponse<AccountResponse>("error", 400, "Mật Khẩu Không Đúng!");
            }

            var newPass = _util.HashPassword(updateAccountPasswordRequest.NewPassword);
            oldAccount.PasswordHash = newPass;

            try
            {
                await _accountRepository.UpdateAccountAsync(oldAccount);

                return new ApiResponse<AccountResponse>("success", "Đổi Mật Khẩu Thành Công!", null);
            }
            catch (Exception)
            {
                return new ApiResponse<AccountResponse>("error", 400, "Đổi Mật Khẩu Thất Bại!");
            }
        }

        //Update Account Status For Admin
        public async Task<ApiResponse<AccountResponse>> UpdateAccountStatusForAdminAsync(int id, UpdateStatusRequest updateStatusRequest)
        {
            var account = await _accountRepository.GetAccountByIdForAdminAsync(id);

            if (account == null)
            {
                return new ApiResponse<AccountResponse>("error", 404, "Tài Khoản Không Tồn Tại!");
            }

            try
            {
                await _accountRepository.UpdateAccountAsync(account);

                return new ApiResponse<AccountResponse>("success", "Cập Nhập Trạng Thái Tài Khoản Thành Công!", null);
            }
            catch (Exception)
            {
                return new ApiResponse<AccountResponse>("error", 400, "Cập Nhập Trạng Thái Tài Khoản Thất Bại!");
            }
        }

        //Update Account To Owner
        public async Task<ApiResponse<BusinessResponseV2>> UpdateAccountToOwnerAsync(int id, CreateBusinessRequest createBusinessRequest)
        {
            var account = await _accountRepository.GetAccountByIdForAdminAsync(id);

            if (account == null)
            {
                return new ApiResponse<BusinessResponseV2>("error", 404, "Tài Khoản Không Tồn Tại!");
            }

            var isPhoneExisted = await _businessRepository.IsPhoneExistedAsync(account.Phone.Trim());

            if (isPhoneExisted)
            {
                return new ApiResponse<BusinessResponseV2>("error", 400, "Số Điện Thoại Đã Được Sử Dụng!");
            }

            var isNameExisted = await _businessRepository.IsNameExistedAsync(createBusinessRequest.Name.Trim().ToLowerInvariant());

            if (isNameExisted)
            {
                return new ApiResponse<BusinessResponseV2>("error", 400, "Tên Doanh Nghiệp Đã Được Sử Dụng!");
            }

            account.Role = RoleConstants.OWNER;

            var newBusiness = _mapper.Map<BusinessProfile>(createBusinessRequest);
            newBusiness.OwnerId = id;
            newBusiness.Name = _util.FormatStringName(createBusinessRequest.Name);

            try
            {
                await _accountRepository.RunInTransactionAsync(async () =>
                {
                    await _accountRepository.UpdateAsync(account);

                    await _businessRepository.CreateBusinessProfileAsync(newBusiness);
                });

                var result = new BusinessResponseV2();
                result.BusinessId = newBusiness.Id;

                return new ApiResponse<BusinessResponseV2>("success", "Cập Nhập Tài Khoản Thành Chủ Nhà Cung Thành Công!", result, 200);
            }
            catch (Exception)
            {
                return new ApiResponse<BusinessResponseV2>("error", 400, "Cập Nhập Tài Khoản Thành Chủ Nhà Cung Thất Bại!");
            }
        }

        public async Task<ApiResponse<IEnumerable<EmployeeResponse>?>> GetEmployeesAsync(GetEmployeeRequest getEmployeeRequest)
        {
            var (accounts, pagination) = await _accountRepository.GetEmployeesAsync(getEmployeeRequest);

            if (accounts == null)
            {
                return new ApiResponse<IEnumerable<EmployeeResponse>?>("error", 404, "Tài Khoản Trống!");
            }

            //Paging caculate
            var data = accounts.Adapt<IEnumerable<EmployeeResponse>>();

            return new ApiResponse<IEnumerable<EmployeeResponse>?>("success", "Lấy dữ liệu thành công!", data, 200, pagination);
        }
    }

}