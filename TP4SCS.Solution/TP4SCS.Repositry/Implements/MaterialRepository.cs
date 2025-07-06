using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Repository.Interfaces;

namespace TP4SCS.Repository.Implements
{
    public class MaterialRepository : GenericRepository<Material>, IMaterialRepository
    {
        public MaterialRepository(Tp4scsDevDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task AddMaterialAsync(int[] branchIds, int businessId, Material material)
        {
            var existingService = await _dbContext.Materials
                            .AnyAsync(s => s.Name.ToLower() == material.Name.ToLower() && s.BranchMaterials.Any(bs => bs.Branch.BusinessId == businessId));

            if (existingService)
            {
                throw new InvalidOperationException($"Material with the name '{material.Name}' already exists for Business ID {businessId}.");
            }
            // Lấy tất cả các branch từ businessId
            var branches = await _dbContext.BusinessBranches
                               .Where(b => b.BusinessId == businessId)
                               .ToListAsync();

            // Thêm material mới vào cơ sở dữ liệu
            await _dbContext.Materials.AddAsync(material);
            await _dbContext.SaveChangesAsync();

            // Kiểm tra trạng thái của material
            if (material.Status.ToLower() == StatusConstants.UNAVAILABLE.ToLower())
            {
                // Tạo BranchMaterial cho mỗi branch với trạng thái UNAVAILABLE
                foreach (var branch in branches)
                {
                    var branchMaterial = new BranchMaterial
                    {
                        MaterialId = material.Id,
                        BranchId = branch.Id,
                        Status = StatusConstants.UNAVAILABLE.ToUpper()
                    };
                    await _dbContext.BranchMaterials.AddAsync(branchMaterial);
                }

                // Lưu lại thay đổi vào cơ sở dữ liệu
                await _dbContext.SaveChangesAsync();
                return; // Kết thúc hàm nếu material là Unavailable
            }

            // Tạo BranchMaterial cho mỗi branch và liên kết với material vừa tạo
            foreach (var branch in branches)
            {
                var branchMaterial = new BranchMaterial
                {
                    MaterialId = material.Id,
                    BranchId = branch.Id,
                    Status = StatusConstants.UNAVAILABLE.ToUpper()
                };

                if (branchIds.Contains(branch.Id) && material.Status.ToLower() == StatusConstants.AVAILABLE.ToLower())
                {
                    branchMaterial.Status = StatusConstants.AVAILABLE.ToUpper();
                }

                await _dbContext.BranchMaterials.AddAsync(branchMaterial);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteMaterialAsync(int id)
        {
            // Lấy danh sách BranchMaterial liên quan đến MaterialId
            var branchMaterials = await _dbContext.BranchMaterials
                                   .Where(bm => bm.MaterialId == id)
                                   .ToListAsync();

            // Xóa tất cả các BranchMaterial nếu có
            if (branchMaterials.Any())
            {
                _dbContext.BranchMaterials.RemoveRange(branchMaterials);
                await _dbContext.SaveChangesAsync();
            }

            // Gọi phương thức DeleteAsync để xóa Material
            await DeleteAsync(id);
        }

        public async Task<Material?> GetMaterialByIdAsync(int id)
        {
            return await _dbContext.Materials// Bao gồm thông tin khuyến mãi nếu có
                .Include(m => m.AssetUrls) // Bao gồm danh sách AssetUrls
                .Include(m => m.BranchMaterials) // Bao gồm BranchMaterials
                .ThenInclude(bm => bm.Branch) // Bao gồm Branch trong BranchMaterials
                .SingleOrDefaultAsync(m => m.Id == id); // Lọc theo Id của Material
        }


        public async Task<IEnumerable<Material>?> GetMaterialsAsync(
            string? keyword = null,
            string? status = null,
            OrderByEnum orderBy = OrderByEnum.IdDesc)
        {
            // Xây dựng bộ lọc
            Expression<Func<Material, bool>> filter = m =>
                (string.IsNullOrEmpty(keyword) || m.Name.Contains(keyword)) &&
                (string.IsNullOrEmpty(status) || m.Status.ToLower().Trim() == status.ToLower().Trim());

            // Bắt đầu truy vấn với bộ lọc
            var query = _dbSet.Where(filter);

            // Áp dụng sắp xếp
            query = orderBy switch
            {
                OrderByEnum.IdDesc => query.OrderByDescending(m => m.Id),
                _ => query.OrderBy(m => m.Id) // Mặc định sắp xếp theo Id tăng dần
            };

            // Bao gồm các thuộc tính liên quan
            query = query
                .Include(m => m.AssetUrls)  // Bao gồm AssetUrls
                .Include(m => m.BranchMaterials) // Bao gồm BranchMaterials
                    .ThenInclude(bm => bm.Branch); // Bao gồm Branch trong BranchMaterials
            return await query.ToListAsync();
        }
        public async Task<IEnumerable<Material>?> GetMaterialsByIdsAsync(List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return null; // Trả về null nếu danh sách ids rỗng hoặc null
            }

            return await _dbContext.Materials
                .Where(m => ids.Contains(m.Id))
                .Include(m => m.AssetUrls)
                .Include(m => m.BranchMaterials)
                .ToListAsync(); // Truy xuất danh sách kết quả
        }
        public async Task UpdateQuantityAsync(int quantity, int branchId, int materialId)
        {
            var branchMaterial = await _dbContext.BranchMaterials
                                                  .Where(bm => bm.MaterialId == materialId && bm.BranchId == branchId)
                                                  .SingleOrDefaultAsync();
            if (branchMaterial == null)
            {
                throw new KeyNotFoundException($"BranchMaterial với MaterialId {materialId} and BranchId {branchId} không tìm thấy.");
            }

            branchMaterial.Storage = quantity;
            _dbContext.BranchMaterials.Update(branchMaterial);
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateMaterialAsync(Material material)
        {
            await UpdateAsync(material);
        }
        public async Task UpdateMaterialAsync(Material material, int[] branchIds)
        {
            var existingBranchMaterials = await _dbContext.BranchMaterials
                                    .Where(bm => bm.MaterialId == material.Id)
                                    .ToListAsync();

            if (material.Status.ToLower() == StatusConstants.UNAVAILABLE.ToLower())
            {
                // Cập nhật trạng thái của tất cả BranchMaterial thành Unavailable
                foreach (var branchMaterial in existingBranchMaterials)
                {
                    branchMaterial.Status = StatusConstants.UNAVAILABLE.ToUpper();
                }

                // Lưu lại thay đổi vào cơ sở dữ liệu
                _dbContext.BranchMaterials.UpdateRange(existingBranchMaterials);
                await _dbContext.SaveChangesAsync();
                return; // Kết thúc hàm nếu material là Unavailable
            }

            // Kiểm tra và cập nhật trạng thái của từng BranchMaterial
            foreach (var branchMaterial in existingBranchMaterials)
            {
                // Kiểm tra xem BranchId có tồn tại trong branchIds hay không
                if (branchIds.Contains(branchMaterial.BranchId))
                {
                    branchMaterial.Status = StatusConstants.AVAILABLE.ToUpper();
                }
                else
                {
                    branchMaterial.Status = StatusConstants.UNAVAILABLE.ToUpper();
                }
            }

            // Cập nhật lại Material trong cơ sở dữ liệu
            await UpdateAsync(material);
        }

    }
}
