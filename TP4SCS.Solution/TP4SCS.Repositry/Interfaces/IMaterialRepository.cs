using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;

namespace TP4SCS.Repository.Interfaces
{
    public interface IMaterialRepository
    {
        // Thêm Material mới và liên kết với các Branch tương ứng
        Task AddMaterialAsync(int[] branchIds, int businessId, Material material);

        // Xóa Material và các BranchMaterial liên quan
        Task DeleteMaterialAsync(int id);

        // Lấy Material theo ID
        Task<Material?> GetMaterialByIdAsync(int id);

        // Lấy danh sách các Material với các bộ lọc và sắp xếp
        Task<IEnumerable<Material>?> GetMaterialsAsync(
            string? keyword = null,
            string? status = null,
            OrderByEnum orderBy = OrderByEnum.IdAsc);

        // Cập nhật thông tin của Material và trạng thái liên quan đến các BranchMaterials
        Task UpdateMaterialAsync(Material material, int[] branchIds);
        Task UpdateQuantityAsync(int quantity, int branchId, int materialId);
        Task UpdateMaterialAsync(Material material);
        Task<IEnumerable<Material>?> GetMaterialsByIdsAsync(List<int> ids);
    }
}
