using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Library.Models.Request.Material;

namespace TP4SCS.Services.Interfaces
{
    public interface IMaterialService
    {
        Task AddMaterialAsync(MaterialCreateRequest materialRequest, int businessId);
        Task<IEnumerable<Material>?> GetMaterialsByIdsAsync(List<int> ids);
        Task DeleteMaterialAsync(int id);

        Task<Material?> GetMaterialByIdAsync(int id);

        Task<(IEnumerable<Material> materials, int total)> GetMaterialsAsync(
            string? keyword = null,
            string? status = null,
            OrderByEnum orderBy = OrderByEnum.IdDesc,
            int pageIndex = 1,
            int pageSize = 10);

        Task UpdateMaterialAsync(MaterialUpdateRequest materialUpdateRequest, int existingMaterialId);

        Task<(IEnumerable<Material>?, int)> GetMaterialsByBranchIdAsync(
            int branchId,
            string? keyword = null,
            string? status = null,
            int? pageIndex = null,
            int? pageSize = null,
            OrderByEnum orderBy = OrderByEnum.IdDesc);

        Task<(IEnumerable<Material>?, int)> GetMaterialsByBusinessIdAsync(
            int businessId,
            string? keyword = null,
            string? status = null,
            int? pageIndex = null,
            int? pageSize = null,
            OrderByEnum orderBy = OrderByEnum.IdDesc);
        Task<(IEnumerable<Material>?, int)> GetMaterialsByServiceIdAsync(
            int serviceId,
            string? keyword = null,
            string? status = null,
            int? pageIndex = null,
            int? pageSize = null,
            OrderByEnum orderBy = OrderByEnum.IdDesc);

        Task UpdateMaterialAsync(int quantity, int branchId, int materialId);
        Task UpdateMaterialAsync(Material material);
    }
}
