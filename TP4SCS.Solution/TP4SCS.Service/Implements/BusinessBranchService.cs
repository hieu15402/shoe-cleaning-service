using Mapster;
using MapsterMapper;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Branch;
using TP4SCS.Library.Models.Request.Business;
using TP4SCS.Library.Models.Response.Branch;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Library.Utils.Utils;
using TP4SCS.Repository.Interfaces;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class BusinessBranchService : IBusinessBranchService
    {
        private readonly IBusinessRepository _businessRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IShipService _shipService;
        private readonly IMapper _mapper;
        private readonly Util _util;

        public BusinessBranchService(IBusinessRepository businessRepository, IBranchRepository branchRepository, IShipService shipService, IMapper mapper, Util util)
        {
            _businessRepository = businessRepository;
            _branchRepository = branchRepository;
            _shipService = shipService;
            _mapper = mapper;
            _util = util;
        }

        //Check Owner Of Branch
        public async Task<bool> CheckOwnerOfBranch(int ownerId, int branchId)
        {
            var idArray = await _branchRepository.GetBranchesIdByOwnerIdAsync(ownerId) ?? Array.Empty<int>();

            return Array.Exists(idArray, id => id == branchId);
        }

        //Create Branch
        public async Task<ApiResponse<BranchResponse>> CreateBranchAsync(int id, HttpClient httpClient, CreateBranchRequest createBranchRequest)
        {
            var wardName = await _shipService.GetWardNameByWardCodeAsync(httpClient, createBranchRequest.DistrictId, createBranchRequest.WardCode) ?? string.Empty;
            var districtName = await _shipService.GetDistrictNamByIdAsync(httpClient, createBranchRequest.DistrictId) ?? string.Empty;
            var provinceName = await _shipService.GetProvinceNameByIdAsync(httpClient, createBranchRequest.ProvinceId) ?? string.Empty;

            var business = await _businessRepository.GetBusinessProfileByIdAsync(id);

            if (business == null)
            {
                return new ApiResponse<BranchResponse>("error", 404, "Tài Khoản Không Sở Hữu Doanh Nghiệp!");
            }

            var newBranch = _mapper.Map<BusinessBranch>(createBranchRequest);
            newBranch.BusinessId = id;
            newBranch.Name = _util.FormatStringName(createBranchRequest.Name);
            newBranch.Address = _util.FormatStringName(createBranchRequest.Address);
            newBranch.Ward = wardName;
            newBranch.District = districtName;
            newBranch.Province = provinceName;

            try
            {
                await _branchRepository.CreateBranchAsync(newBranch);

                var newId = await GetBranchMaxIdAsync();

                var newBr = await GetBranchByIdAsync(newId);

                return new ApiResponse<BranchResponse>("success", "Tạo Chi Nhánh Thành Công!", newBr.Data, 201);
            }
            catch (Exception)
            {
                return new ApiResponse<BranchResponse>("error", 400, "Tạo Chi Nhánh Thất Bại!");
            }
        }

        //Create Branch By Onwer Id
        public async Task<ApiResponse<BranchResponse>> CreateBranchByOwnerIdAsync(int id, HttpClient httpClient, CreateBranchRequest createBranchRequest)
        {
            var wardName = await _shipService.GetWardNameByWardCodeAsync(httpClient, createBranchRequest.DistrictId, createBranchRequest.WardCode) ?? string.Empty;
            var districtName = await _shipService.GetDistrictNamByIdAsync(httpClient, createBranchRequest.DistrictId) ?? string.Empty;
            var provinceName = await _shipService.GetProvinceNameByIdAsync(httpClient, createBranchRequest.ProvinceId) ?? string.Empty;

            var businessId = await _businessRepository.GetBusinessIdByOwnerIdAsync(id);

            if (businessId == null)
            {
                return new ApiResponse<BranchResponse>("error", 404, "Tài Khoản Không Sở Hữu Doanh Nghiệp!");
            }

            var newBranch = _mapper.Map<BusinessBranch>(createBranchRequest);
            newBranch.BusinessId = (int)businessId;
            newBranch.Name = _util.FormatStringName(createBranchRequest.Name);
            newBranch.Address = _util.FormatStringName(createBranchRequest.Address);
            newBranch.Ward = wardName;
            newBranch.District = districtName;
            newBranch.Province = provinceName;

            try
            {
                await _branchRepository.CreateBranchAsync(newBranch);

                var newId = await GetBranchMaxIdAsync();

                var newBr = await GetBranchByIdAsync(newId);

                return new ApiResponse<BranchResponse>("success", "Tạo Chi Nhánh Thành Công!", newBr.Data);
            }
            catch (Exception)
            {
                return new ApiResponse<BranchResponse>("error", 400, "Tạo Chi Nhánh Thất Bại!");
            }
        }

        //Get Branch By Id
        public async Task<ApiResponse<BranchResponse?>> GetBranchByIdAsync(int id)
        {
            var branch = await _branchRepository.GetBranchByIdAsync(id);

            if (branch == null)
            {
                return new ApiResponse<BranchResponse?>("error", 404, "Không Tìm Thấy Chi Nhánh!");
            }

            var data = _mapper.Map<BranchResponse>(branch);

            return new ApiResponse<BranchResponse?>("success", "Lấy Chi Nhánh Thành Công!", data);
        }

        //Get Barnch By Business Id
        public async Task<ApiResponse<IEnumerable<BranchResponse>?>> GetBranchesByBusinessIdAsync(int id)
        {
            var branches = await _branchRepository.GetBranchesByBusinessIdAsync(id);

            if (branches == null)
            {
                return new ApiResponse<IEnumerable<BranchResponse>?>("error", 404, "Không Tìm Thấy Chi Nhánh!");
            }

            var data = branches.Adapt<IEnumerable<BranchResponse>>();

            return new ApiResponse<IEnumerable<BranchResponse>?>("success", "Lấy Chi Nhánh Thành Công!", data);
        }

        //Get Branch Max Id
        public async Task<int> GetBranchMaxIdAsync()
        {
            return await _branchRepository.GetBranchMaxIdAsync();
        }

        //Update Branch
        public async Task<ApiResponse<BranchResponse>> UpdateBranchAsync(int id, HttpClient httpClient, UpdateBranchRequest updateBranchRequest)
        {
            var wardName = await _shipService.GetWardNameByWardCodeAsync(httpClient, updateBranchRequest.DistrictId, updateBranchRequest.WardCode) ?? string.Empty;
            var districtName = await _shipService.GetDistrictNamByIdAsync(httpClient, updateBranchRequest.DistrictId) ?? string.Empty;
            var provinceName = await _shipService.GetProvinceNameByIdAsync(httpClient, updateBranchRequest.ProvinceId) ?? string.Empty;

            var oldBranch = await _branchRepository.GetBranchByIdAsync(id);

            if (oldBranch == null)
            {
                return new ApiResponse<BranchResponse>("error", 404, "Chi Nhánh Không Tồn Tại!");
            }

            var newBranch = _mapper.Map(updateBranchRequest, oldBranch);
            newBranch.Name = _util.FormatStringName(updateBranchRequest.Name);
            newBranch.Address = _util.FormatStringName(updateBranchRequest.Address);
            newBranch.Ward = wardName;
            newBranch.District = districtName;
            newBranch.Province = provinceName;
            newBranch.Status = updateBranchRequest.Status switch
            {
                BranchStatus.INACTIVE => StatusConstants.INACTIVE,
                _ => StatusConstants.ACTIVE
            };

            try
            {
                await _branchRepository.UpdateBranchAsync(newBranch);

                return new ApiResponse<BranchResponse>("success", "Cập Nhập Chi Nhánh Thành Công!", null);
            }
            catch (Exception)
            {
                return new ApiResponse<BranchResponse>("error", 400, "Cập Nhập Chi Nhánh Thất Bại!");
            }
        }

        //Update Branch Employee
        public async Task<ApiResponse<BranchResponse>> UpdateBranchEmployeeAsync(int id, UpdateBranchEmployeeRequest updateBranchEmployeeRequest)
        {
            var oldBranch = await _branchRepository.GetByIDAsync(id);

            if (oldBranch == null)
            {
                return new ApiResponse<BranchResponse>("error", 404, "Chi Nhánh Không Tồn Tại!");
            }

            if (updateBranchEmployeeRequest.IsDeleted)
            {
                var deleteError = _util.CheckDeleteEmployeesErrorType(oldBranch.EmployeeIds, updateBranchEmployeeRequest.EmployeeIds);

                var deleteErrorMessage = new Dictionary<string, string>
                {
                    { "Empty", "Chi Nhánh Chưa Có Nhân Viên Nào!" },
                    { "Null", "Nhân Viên Không Thuộc Chi Nhánh!" },
                    { "Invalid", "Trường Nhập Không Khả Dụng!" },
                };

                if (deleteErrorMessage.TryGetValue(deleteError, out var errorMessage))
                {
                    return new ApiResponse<BranchResponse>("error", 400, errorMessage);
                }

                oldBranch.EmployeeIds = _util.DeleteEmployeesId(oldBranch.EmployeeIds, updateBranchEmployeeRequest.EmployeeIds);
            }
            else
            {
                var addError = _util.CheckAddEmployeesErrorType(oldBranch.EmployeeIds, updateBranchEmployeeRequest.EmployeeIds);

                var addErrorMessage = new Dictionary<string, string>
                {
                    { "Full", "Chi Nhánh Đã Đạt Tối Đa 5 Nhân Viên!" },
                    { "Existed", "Nhân Viên Đã Thuộc Chi Nhánh!" },
                    { "Invalid", "Trường Nhập Không Khả Dụng!" },
                };

                if (addErrorMessage.TryGetValue(addError, out var message))
                {
                    return new ApiResponse<BranchResponse>("error", 400, message);
                }

                oldBranch.EmployeeIds = _util.AddEmployeeId(oldBranch.EmployeeIds, updateBranchEmployeeRequest.EmployeeIds);
            }

            try
            {
                await _branchRepository.UpdateBranchAsync(oldBranch);

                return new ApiResponse<BranchResponse>("success", "Cập Nhập Nhân Viên Chi Nhánh Thành Công!", null);
            }
            catch (Exception)
            {
                return new ApiResponse<BranchResponse>("error", 400, "Cập Nhập Nhân Viên Chi Nhánh Thất Bại!");
            }
        }

        //Update Branch Statistic
        public async Task<ApiResponse<BranchResponse>> UpdateBranchStatisticAsync(int id, UpdateBranchStatisticRequest updateBranchStatisticRequest)
        {
            var branch = await _branchRepository.GetBranchByIdAsync(id);

            if (branch == null)
            {
                return new ApiResponse<BranchResponse>("error", 404, "Chi Nhánh Không Tồn Tại!");
            }

            switch (updateBranchStatisticRequest.Type)
            {
                case OrderStatistic.PENDING:
                    branch.PendingAmount += 1;
                    break;
                case OrderStatistic.PROCESSING:
                    branch.PendingAmount = (branch.PendingAmount - 1 < 0) ? 0 : branch.PendingAmount - 1;
                    branch.ProcessingAmount += 1;
                    break;
                case OrderStatistic.FINISHED:
                    branch.ProcessingAmount = (branch.ProcessingAmount - 1 < 0) ? 0 : branch.ProcessingAmount - 1;
                    branch.FinishedAmount += 1;
                    break;
                case OrderStatistic.CANCELED:
                    branch.PendingAmount = (branch.PendingAmount - 1 < 0) ? 0 : branch.PendingAmount - 1;
                    branch.CanceledAmount += 1;
                    break;
                default:
                    break;
            }

            try
            {
                await _branchRepository.UpdateBranchAsync(branch);

                return new ApiResponse<BranchResponse>("success", "Cập Nhập Thống Kê Chi Nhánh Thành Công!", null);
            }
            catch (Exception)
            {
                return new ApiResponse<BranchResponse>("error", 400, "Cập Nhập Thống Kê Chi Nhánh Thất Bại!");
            }
        }

        //Update Branch Status
        public async Task<ApiResponse<BranchResponse>> UpdateBranchStatusForAdminAsync(int id, UpdateBranchStatusRequest updateBranchStatusRequest)
        {
            var oldBranch = await _branchRepository.GetBranchByIdAsync(id);

            if (oldBranch == null)
            {
                return new ApiResponse<BranchResponse>("error", 404, "Chi Nhánh Không Tồn Tại!");
            }

            var newStatus = updateBranchStatusRequest.Status switch
            {
                BranchStatus.INACTIVE => StatusConstants.INACTIVE,
                BranchStatus.ACTIVE => StatusConstants.ACTIVE,
                _ => StatusConstants.SUSPENDED
            };

            if (!oldBranch.Status.Equals(newStatus))
            {

                return new ApiResponse<BranchResponse>("error", 400, "Trạng Thái Chi Nhánh Trùng Lập!");
            }

            oldBranch.Status = newStatus;

            try
            {
                await _branchRepository.UpdateBranchAsync(oldBranch);

                return new ApiResponse<BranchResponse>("success", "Cập Nhập Chi Nhánh Thành Công!", null);
            }
            catch (Exception)
            {
                return new ApiResponse<BranchResponse>("error", 400, "Cập Nhập Chi Nhánh Thất Bại!");
            }
        }
    }
}
