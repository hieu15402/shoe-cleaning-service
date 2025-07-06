using TP4SCS.Library.Models.Request.Branch;
using TP4SCS.Library.Models.Response.Branch;
using TP4SCS.Library.Models.Response.General;

namespace TP4SCS.Services.Interfaces
{
    public interface IBusinessBranchService
    {
        Task<ApiResponse<IEnumerable<BranchResponse>?>> GetBranchesByBusinessIdAsync(int id);

        Task<ApiResponse<BranchResponse?>> GetBranchByIdAsync(int id);

        Task<int> GetBranchMaxIdAsync();

        Task<ApiResponse<BranchResponse>> CreateBranchByOwnerIdAsync(int id, HttpClient httpClient, CreateBranchRequest createBranchRequest);

        Task<ApiResponse<BranchResponse>> CreateBranchAsync(int id, HttpClient httpClient, CreateBranchRequest createBranchRequest);

        Task<ApiResponse<BranchResponse>> UpdateBranchAsync(int id, HttpClient httpClient, UpdateBranchRequest updateBranchRequest);

        Task<ApiResponse<BranchResponse>> UpdateBranchEmployeeAsync(int id, UpdateBranchEmployeeRequest updateBranchEmployeeRequest);

        Task<ApiResponse<BranchResponse>> UpdateBranchStatisticAsync(int id, UpdateBranchStatisticRequest updateBranchStatisticRequest);

        Task<ApiResponse<BranchResponse>> UpdateBranchStatusForAdminAsync(int id, UpdateBranchStatusRequest updateBranchStatus);

        Task<bool> CheckOwnerOfBranch(int OwnerId, int BranchId);
    }
}