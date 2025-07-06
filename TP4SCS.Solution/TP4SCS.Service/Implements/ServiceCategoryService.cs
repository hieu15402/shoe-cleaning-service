using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Library.Utils.Utils;
using TP4SCS.Repository.Interfaces;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class ServiceCategoryService : IServiceCategoryService
    {
        private IServiceCategoryRepository _categoryRepository;

        public ServiceCategoryService(IServiceCategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task AddServiceCategoryAsync(ServiceCategory category)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category), "Danh mục không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                throw new ArgumentException("Tên danh mục là bắt buộc.");
            }
            if (category.Name.Length < 3 || category.Name.Length > 100)
            {
                throw new ArgumentException("Tên danh mục phải nằm trong khoảng từ 3 đến 100 ký tự.");
            }
            if (!Util.IsValidGeneralStatus(category.Status))
            {
                throw new ArgumentException("Status không hợp lệ.");
            }
            category.Status = category.Status.ToUpper().Trim();
            await _categoryRepository.AddCategoryAsync(category);
        }
        public async Task<int> GetTotalServiceCategoriesCountAsync(string? keyword = null, string? status = null)
        {
            return await _categoryRepository.GetTotalCategoriesCountAsync(keyword, status);
        }
        public async Task DeleteServiceCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                throw new Exception($"Danh mục với ID {id} không tìm thấy.");
            }
            await _categoryRepository.DeleteCategoryAsync(id);
        }

        public async Task<IEnumerable<ServiceCategory>?> GetServiceCategoriesAsync(string? keyword = null, string? status = null,
            int pageIndex = 1, int pageSize = 5, OrderByEnum orderBy = OrderByEnum.IdAsc)
        {
            if (pageIndex < 1)
            {
                throw new ArgumentException("Chỉ số trang phải lớn hơn 0.");
            }

            if (pageSize < 1)
            {
                throw new ArgumentException("Kích thước trang phải lớn hơn 0.");
            }
            return await _categoryRepository.GetCategoriesAsync(keyword, status, pageIndex, pageSize, orderBy);
        }

        public async Task<ServiceCategory?> GetServiceCategoryByIdAsync(int id)
        {
            return await _categoryRepository.GetCategoryByIdAsync(id);
        }

        public async Task UpdateServiceCategoryAsync(ServiceCategory category, int existingCategoryId)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category), "Danh mục không được để trống.");
            }
            if (string.IsNullOrWhiteSpace(category.Name))
            {
                throw new ArgumentException("Tên danh mục là bắt buộc.");
            }
            if (category.Name.Length < 3 || category.Name.Length > 100)
            {
                throw new ArgumentException("Tên danh mục phải nằm trong khoảng từ 3 đến 100 ký tự.");
            }
            if (string.IsNullOrEmpty(category.Status) || !Util.IsValidGeneralStatus(category.Status))
            {
                throw new ArgumentException("Status của Category không hợp lệ.");
            }
            var existingCategory = await _categoryRepository.GetCategoryByIdAsync(existingCategoryId);
            if (existingCategory == null)
            {
                throw new KeyNotFoundException($"Danh mục với ID {existingCategoryId} không tìm thấy.");
            }
            existingCategory.Name = category.Name;
            existingCategory.Status = category.Status;

            await _categoryRepository.UpdateCategoryAsync(existingCategory);
        }
    }
}
