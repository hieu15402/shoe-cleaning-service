using AutoMapper;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Library.Models.Request.Material;
using TP4SCS.Library.Utils.Utils;
using TP4SCS.Repository.Interfaces;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class MaterialService : IMaterialService
    {
        private readonly IMaterialRepository _materialRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IAssetUrlService _assetUrlService;
        private readonly IMapper _mapper;

        public MaterialService(IMaterialRepository materialRepository,
            IAssetUrlService assetUrlService,
            IMapper mapper,
            IBusinessRepository businessRepository)
        {
            _materialRepository = materialRepository;
            _assetUrlService = assetUrlService;
            _mapper = mapper;
            _businessRepository = businessRepository;
        }

        public async Task AddMaterialAsync(MaterialCreateRequest materialRequest, int businessId)
        {
            var business = await _businessRepository.GetByIDAsync(businessId);
            if (business == null)
            {
                throw new ArgumentNullException($"Không tìm thấy business với id: {businessId}.");
            }
            if (business.IsMaterialSupported == false)
            {
                throw new ArgumentNullException($"Cửa hàng {business.Name} vui lòng nâng cấp gói đăng ký để thêm mới phụ kiện.");
            }
            if (materialRequest == null)
            {
                throw new ArgumentNullException(nameof(materialRequest), "Yêu cầu thêm nguyên liệu không được để trống.");
            }

            if (materialRequest.Price <= 0)
            {
                throw new ArgumentException("Giá phải lớn hơn 0.");
            }

            if (materialRequest.AssetUrls == null || !materialRequest.AssetUrls.Any())
            {
                throw new ArgumentException("Hình ảnh không được để trống.");
            }

            var material = _mapper.Map<Material>(materialRequest);

            await _materialRepository.AddMaterialAsync(materialRequest.BranchId, businessId, material);
        }
        public async Task DeleteMaterialAsync(int id)
        {
            var material = await _materialRepository.GetMaterialByIdAsync(id);

            if (material == null)
            {
                throw new Exception($"Nguyên liệu với ID {id} không tìm thấy.");
            }

            // Xóa AssetUrls nếu tồn tại
            if (material.AssetUrls != null)
            {
                var assetUrlsToDelete = material.AssetUrls.ToList();
                foreach (var assetUrl in assetUrlsToDelete)
                {
                    await _assetUrlService.DeleteAssetUrlAsync(assetUrl.Id);
                }
            }

            // Xóa Material
            await _materialRepository.DeleteMaterialAsync(id);
        }
        public async Task<Material?> GetMaterialByIdAsync(int id)
        {
            var material = await _materialRepository.GetMaterialByIdAsync(id);
            return material;
        }
        public async Task<(IEnumerable<Material> materials, int total)> GetMaterialsAsync(
            string? keyword = null,
            string? status = null,
            OrderByEnum orderBy = OrderByEnum.IdDesc,
            int pageIndex = 1,
            int pageSize = 10)
        {
            if (pageIndex <= 0 || pageSize <= 0)
            {
                throw new ArgumentException("PageIndex và PageSize phải lớn hơn 0.");
            }

            var materials = await _materialRepository.GetMaterialsAsync(keyword, status, orderBy);

            if (materials == null || !materials.Any())
            {
                return (Enumerable.Empty<Material>(), 0);
            }
            var total = materials.Count();

            // Phân trang
            var paginatedMaterials = materials
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);

            return (paginatedMaterials, total);
        }
        public async Task<IEnumerable<Material>?> GetMaterialsByIdsAsync(List<int> ids)
        {
            return await _materialRepository.GetMaterialsByIdsAsync(ids);
        }
        public async Task UpdateMaterialAsync(MaterialUpdateRequest materialUpdateRequest, int existingMaterialId)
        {
            if (materialUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(materialUpdateRequest), "Yêu cầu vật liệu không được để trống.");
            }

            if (materialUpdateRequest.Price <= 0)
            {
                throw new ArgumentException("Giá phải lớn hơn 0.");
            }

            if (string.IsNullOrEmpty(materialUpdateRequest.Status) || !Util.IsValidGeneralStatus(materialUpdateRequest.Status))
            {
                throw new ArgumentException("Trạng thái của Material không hợp lệ.", nameof(materialUpdateRequest.Status));
            }

            var existingMaterial = await _materialRepository.GetMaterialByIdAsync(existingMaterialId);
            if (existingMaterial == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy vật liệu nào với ID {existingMaterialId}.");
            }

            existingMaterial.Name = materialUpdateRequest.Name;
            existingMaterial.Price = materialUpdateRequest.Price;
            existingMaterial.Status = materialUpdateRequest.Status;

            // Xử lý AssetUrls (nếu có)
            var existingAssetUrls = existingMaterial.AssetUrls.ToList();
            var newAssetUrls = materialUpdateRequest.AssetUrls;

            var newUrls = newAssetUrls.Select(a => a.Url).ToList();

            // Xóa các URL không còn tồn tại
            var urlsToRemove = existingAssetUrls.Where(a => !newUrls.Contains(a.Url)).ToList();
            if (urlsToRemove.Any())
            {
                foreach (var assetUrl in urlsToRemove)
                {
                    await _assetUrlService.DeleteAssetUrlAsync(assetUrl.Id);
                    existingMaterial.AssetUrls.Remove(assetUrl);
                }
            }

            // Thêm các URL mới
            var urlsToAdd = newAssetUrls.Where(a => !existingAssetUrls.Any(e => e.Url == a.Url)).ToList();
            if (urlsToAdd.Any())
            {
                foreach (var newAsset in urlsToAdd)
                {
                    var newAssetUrl = new AssetUrl
                    {
                        Url = newAsset.Url,
                        Type = newAsset.Type
                    };
                    existingMaterial.AssetUrls.Add(newAssetUrl);
                }
            }

            // Cập nhật Material
            await _materialRepository.UpdateMaterialAsync(existingMaterial, materialUpdateRequest.BranchId);
        }

        public async Task UpdateMaterialAsync(Material material)
        {
            await _materialRepository.UpdateMaterialAsync(material);
        }

        public async Task<(IEnumerable<Material>?, int)> GetMaterialsByBranchIdAsync(
            int branchId,
            string? keyword = null,
            string? status = null,
            int? pageIndex = null,
            int? pageSize = null,
            OrderByEnum orderBy = OrderByEnum.IdDesc)
        {
            // Chỉ lấy danh sách vật liệu theo keyword và status từ repository
            var materials = await _materialRepository.GetMaterialsAsync(keyword, status, orderBy);

            // Lọc các vật liệu theo branchId
            var filteredMaterials = materials?.Where(m => m.BranchMaterials.Any(bm => bm.BranchId == branchId));

            // Đếm tổng số vật liệu sau khi lọc
            int totalCount = filteredMaterials?.Count() ?? 0;

            // Thực hiện phân trang nếu pageIndex và pageSize có giá trị
            if (pageIndex.HasValue && pageSize.HasValue && pageSize > 0)
            {
                filteredMaterials = filteredMaterials?
                    .Skip((pageIndex.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            }

            return (filteredMaterials, totalCount);
        }

        public async Task<(IEnumerable<Material>?, int)> GetMaterialsByServiceIdAsync(
            int serviceId,
            string? keyword = null,
            string? status = null,
            int? pageIndex = null,
            int? pageSize = null,
            OrderByEnum orderBy = OrderByEnum.IdDesc)
        {
            // Chỉ lấy danh sách vật liệu theo keyword và status từ repository
            var materials = await _materialRepository.GetMaterialsAsync(keyword, status, orderBy);

            // Lọc các vật liệu theo businessId thông qua BranchMaterials -> Branch -> Business
            var filteredMaterials = materials?.Where(m => m.ServiceId == serviceId);

            // Đếm tổng số vật liệu sau khi lọc
            int totalCount = filteredMaterials?.Count() ?? 0;

            // Thực hiện phân trang nếu pageIndex và pageSize có giá trị
            if (pageIndex.HasValue && pageSize.HasValue && pageSize > 0)
            {
                filteredMaterials = filteredMaterials?
                    .Skip((pageIndex.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            }

            return (filteredMaterials, totalCount);
        }

        public async Task<(IEnumerable<Material>?, int)> GetMaterialsByBusinessIdAsync(
            int businessId,
            string? keyword = null,
            string? status = null,
            int? pageIndex = null,
            int? pageSize = null,
            OrderByEnum orderBy = OrderByEnum.IdDesc)
        {
            // Chỉ lấy danh sách vật liệu theo keyword và status từ repository
            var materials = await _materialRepository.GetMaterialsAsync(keyword, status, orderBy);

            // Lọc các vật liệu theo businessId thông qua BranchMaterials -> Branch -> Business
            var filteredMaterials = materials?.Where(m =>
                m.BranchMaterials.Any(bm => bm.Branch.BusinessId == businessId));

            // Đếm tổng số vật liệu sau khi lọc
            int totalCount = filteredMaterials?.Count() ?? 0;

            // Thực hiện phân trang nếu pageIndex và pageSize có giá trị
            if (pageIndex.HasValue && pageSize.HasValue && pageSize > 0)
            {
                filteredMaterials = filteredMaterials?
                    .Skip((pageIndex.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            }

            return (filteredMaterials, totalCount);
        }

        public async Task UpdateMaterialAsync(int quantity, int branchId, int materialId)
        {
            await _materialRepository.UpdateQuantityAsync(quantity, branchId, materialId);
        }
    }
}
