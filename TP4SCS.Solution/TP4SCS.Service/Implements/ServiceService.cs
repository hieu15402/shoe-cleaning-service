using AutoMapper;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Library.Models.Request.Service;
using TP4SCS.Library.Models.Response.Service;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Library.Utils.Utils;
using TP4SCS.Repository.Interfaces;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class ServiceService : IServiceService
    {
        private readonly IServiceRepository _serviceRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IMapper _mapper;
        private readonly IServiceCategoryRepository _categoryRepository;
        private readonly IPromotionService _promotionService;
        private readonly IAssetUrlService _assetUrlService;
        private readonly IProcessService _processService;

        public ServiceService(IServiceRepository serviceRepository,
            IMapper mapper,
            IServiceCategoryRepository categoryRepository,
            IPromotionService promotionService,
            IAssetUrlService assetUrlService,
            IProcessService processService,
            IBusinessRepository businessRepository)
        {
            _serviceRepository = serviceRepository;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
            _promotionService = promotionService;
            _assetUrlService = assetUrlService;
            _processService = processService;
            _businessRepository = businessRepository;
        }

        public async Task AddServiceAsync(ServiceCreateRequest serviceRequest, int businessId)
        {
            var business = await _businessRepository.GetByIDAsync(businessId);
            if (business == null)
            {
                throw new ArgumentNullException($"Không tìm thấy business với id: {businessId}.");
            }
            if (business.IsLimitServiceNum)
            {
                var services = await _serviceRepository.GetServicesAsync(null, null, null, null, OrderByEnum.IdDesc);
                var count = services?.Where(s => s.BranchServices.Any(bs => bs.Branch.BusinessId == businessId)).Count() ?? 0;
                if (count >= 5) throw new ArgumentNullException($"Cửa hàng {business.Name} vui lòng nâng cấp gói đăng ký để thêm mới dịch vụ.");
            }
            if (serviceRequest == null)
            {
                throw new ArgumentNullException(nameof(serviceRequest), "Yêu cầu dịch vụ không được để trống.");
            }

            if (serviceRequest.Price <= 0)
            {
                throw new ArgumentException("Giá phải lớn hơn 0.");
            }

            if (serviceRequest.NewPrice.HasValue && serviceRequest.NewPrice <= 0)
            {
                throw new ArgumentException("Giá giảm phải lớn hơn 0.");
            }

            if (serviceRequest.NewPrice.HasValue && serviceRequest.NewPrice > serviceRequest.Price)
            {
                throw new ArgumentException("Giá sau khi giảm phải bé hơn hoặc bằng giá gốc.");
            }
            if (serviceRequest.AssetUrls == null)
            {
                throw new ArgumentException("Hình ảnh không được để trống.");
            }

            var category = await _categoryRepository.GetCategoryByIdAsync(serviceRequest.CategoryId);

            if (category == null)
            {
                throw new ArgumentException("ID danh mục không hợp lệ.");
            }

            if (Util.IsEqual(category.Status, StatusConstants.UNAVAILABLE))
            {
                throw new ArgumentException("Danh mục này đã ngưng hoạt động.");
            }


            var service = _mapper.Map<Service>(serviceRequest);
            service.CreateTime = DateTime.Now;

            if (serviceRequest.NewPrice.HasValue)
            {
                service.Promotion = new Promotion
                {
                    NewPrice = serviceRequest.NewPrice.Value,
                    SaleOff = 100 - (int)Math.Round((serviceRequest.NewPrice.Value / serviceRequest.Price * 100), MidpointRounding.AwayFromZero),
                    Status = StatusConstants.AVAILABLE
                };

                if (string.IsNullOrEmpty(service.Promotion.Status) || !Util.IsValidGeneralStatus(service.Promotion.Status))
                {
                    throw new ArgumentException("Status của Promotion không hợp lệ.");
                }
            }
            else
            {
                service.Promotion = null;
            }

            await _serviceRepository.AddServiceAsync(serviceRequest.BranchId, businessId, service);
            business.ToTalServiceNum++;
            await _businessRepository.UpdateAsync(business);
        }

        public async Task DeleteServiceAsync(int id)
        {
            var service = await _serviceRepository.GetServiceByIdAsync(id);

            if (service == null)
            {
                throw new Exception($"Dịch vụ với ID {id} không tìm thấy.");
            }

            if (service.Promotion != null)
            {
                await _promotionService.DeletePromotionAsync(service.Promotion.Id);
            }
            if (service.AssetUrls != null)
            {
                var assetUrlsToDelete = service.AssetUrls.ToList();
                foreach (var assetUrl in assetUrlsToDelete)
                {
                    await _assetUrlService.DeleteAssetUrlAsync(assetUrl.Id);
                }
            }

            await _serviceRepository.DeleteServiceAsync(id);
        }

        public async Task<Service?> GetServiceByIdAsync(int id)
        {
            var service = await _serviceRepository.GetServiceByIdAsync(id);
            return service;
        }

        public async Task<IEnumerable<Service>?> GetServicesAsync(
            string? keyword = null,
            string? status = null,
            int pageIndex = 1,
            int pageSize = 10,
            OrderByEnum orderBy = OrderByEnum.IdAsc)
        {
            if (pageIndex < 1)
            {
                throw new ArgumentException("Chỉ số trang phải lớn hơn 0.");
            }

            if (pageSize < 1)
            {
                throw new ArgumentException("Kích thước trang phải lớn hơn 0.");
            }

            return await _serviceRepository.GetServicesAsync(keyword, status, pageIndex, pageSize, orderBy);
        }
        public async Task<IEnumerable<ServiceResponseV3>?> GetServicesIncludeBusinessRankAsync(
            string? keyword = null,
            string? status = null,
            int pageIndex = 1,
            int pageSize = 10,
            OrderByEnum orderBy = OrderByEnum.IdAsc)
        {
            if (pageIndex < 1)
            {
                throw new ArgumentException("Chỉ số trang phải lớn hơn 0.");
            }

            if (pageSize < 1)
            {
                throw new ArgumentException("Kích thước trang phải lớn hơn 0.");
            }

            return await _serviceRepository.GetServicesIncludeBusinessRankAsync(keyword, status, pageIndex, pageSize, orderBy);
        }
        public async Task UpdateServiceAsync(ServiceUpdateRequest serviceUpdateRequest, int existingServiceId)
        {
            if (serviceUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(serviceUpdateRequest), "Yêu cầu dịch vụ không được để trống.");
            }

            if (serviceUpdateRequest.Price <= 0)
            {
                throw new ArgumentException("Giá phải lớn hơn 0.");
            }
            if (string.IsNullOrEmpty(serviceUpdateRequest.Status) || !Util.IsValidGeneralStatus(serviceUpdateRequest.Status))
            {
                throw new ArgumentException("Status của Service không hợp lệ.", nameof(serviceUpdateRequest.Status));
            }
            var category = await _categoryRepository.GetCategoryByIdAsync(serviceUpdateRequest.CategoryId);
            if (category == null)
            {
                throw new ArgumentException("ID danh mục không hợp lệ.");
            }

            if (Util.IsEqual(category.Status, StatusConstants.INACTIVE))
            {
                throw new ArgumentException("Danh mục này đã ngưng hoạt động.");
            }
            if (serviceUpdateRequest.NewPrice.HasValue &&
                serviceUpdateRequest.NewPrice > serviceUpdateRequest.Price)
            {
                throw new ArgumentException("Giá sau khi giảm phải bé hơn hoặc bằng giá gốc.");
            }
            var existingService = await _serviceRepository.GetServiceByIdAsync(existingServiceId);
            if (existingService == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy dịch vụ nào.");
            }

            existingService.Name = serviceUpdateRequest.Name;
            existingService.CategoryId = serviceUpdateRequest.CategoryId;
            existingService.Description = serviceUpdateRequest.Description ?? "";
            existingService.Price = serviceUpdateRequest.Price;
            existingService.Status = serviceUpdateRequest.Status;

            if (serviceUpdateRequest.NewPrice.HasValue)
            {
                if (existingService.Promotion != null)
                {
                    existingService.Promotion.NewPrice = serviceUpdateRequest.NewPrice.Value;
                    existingService.Promotion.Status = StatusConstants.AVAILABLE;

                    await _promotionService.UpdatePromotionAsync(existingService.Promotion, existingService.Promotion.Id);
                }
                else
                {
                    var newPromotion = new Promotion
                    {
                        ServiceId = existingService.Id,
                        NewPrice = serviceUpdateRequest.NewPrice.Value,
                        Status = StatusConstants.AVAILABLE
                    };
                    await _promotionService.AddPromotionAsync(newPromotion);
                }
            }
            else if (!serviceUpdateRequest.NewPrice.HasValue)
            {
                if (existingService.Promotion != null)
                {
                    existingService.Promotion.NewPrice = existingService.Price;
                    existingService.Promotion.Status = StatusConstants.UNAVAILABLE;

                }
            }

            var existingAssetUrls = existingService.AssetUrls.ToList();
            var newAssetUrls = serviceUpdateRequest.AssetUrls;

            var newUrls = newAssetUrls.Select(a => a.Url).ToList();

            var urlsToRemove = existingService.AssetUrls.Where(a => !newUrls.Contains(a.Url)).ToList();
            if (urlsToRemove.Any())
            {
                foreach (var assetUrl in urlsToRemove)
                {
                    await _assetUrlService.DeleteAssetUrlAsync(assetUrl.Id);
                    existingService.AssetUrls.Remove(assetUrl);
                }
            }

            var urlsToAdd = newAssetUrls.Where(a => !existingAssetUrls.Any(e => e.Url == a.Url)).ToList();
            if (urlsToAdd.Any())
            {
                foreach (var newAsset in urlsToAdd)
                {
                    var newAssetUrl = new AssetUrl
                    {
                        Url = newAsset.Url,
                        //IsImage = newAsset.IsImage,
                        Type = newAsset.Type
                    };
                    existingService.AssetUrls.Add(newAssetUrl);
                }
            }

            var existingProcesses = existingService.ServiceProcesses.ToList();
            var newProcesses = serviceUpdateRequest.ServiceProcesses;

            // Các Process mới cần thêm vào
            var processesToAdd = newProcesses.Where(newProcess =>
                !existingProcesses.Any(existingProcess =>
                    existingProcess.Process == newProcess.Process &&
                    existingProcess.ProcessOrder == newProcess.ProcessOrder)
            ).ToList();

            // Các Process cần xóa
            var processesToRemove = existingProcesses.Where(existingProcess =>
                !newProcesses.Any(newProcess =>
                    newProcess.Process == existingProcess.Process &&
                    newProcess.ProcessOrder == existingProcess.ProcessOrder)
            ).ToList();

            // Xử lý thêm mới các Process
            if (processesToAdd.Any())
            {
                foreach (var newProcess in processesToAdd)
                {
                    existingService.ServiceProcesses.Add(new ServiceProcess
                    {
                        Process = newProcess.Process,
                        ProcessOrder = newProcess.ProcessOrder
                    });
                }
            }

            // Xử lý xóa các Process
            if (processesToRemove.Any())
            {
                foreach (var processToRemove in processesToRemove)
                {
                    await _processService.DeleteProcessAsync(processToRemove.Id);
                }
            }

            await _serviceRepository.UpdateServiceAsync(existingService, serviceUpdateRequest.BranchId);

        }

        public async Task<(IEnumerable<Service>?, int)> GetDiscountedServicesAsync(
            string? name = null,
            string? status = null,
            int? pageIndex = null,
            int? pageSize = null)
        {
            // Lấy tất cả dịch vụ thỏa mãn điều kiện khuyến mãi trước khi phân trang
            var allServices = await _serviceRepository.GetServicesAsync(name, status, null, null, OrderByEnum.IdAsc);

            // Lọc các dịch vụ có khuyến mãi và trạng thái khuyến mãi là "Available"
            var discountedServices = allServices?.Where(service =>
                service.Promotion != null &&
                Util.IsEqual(service.Promotion.Status, StatusConstants.AVAILABLE)
            );

            // Tính tổng số lượng dịch vụ khuyến mãi
            var totalCount = discountedServices?.Count() ?? 0;

            // Áp dụng phân trang sau khi đã tính tổng số lượng
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10;
                discountedServices = discountedServices?.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            return (discountedServices, totalCount);
        }
        public async Task<(IEnumerable<ServiceResponseV3>?, int)> GetDiscountedServicesIncludeBusinessRankAsync(
            string? name = null,
            string? status = null,
            int? pageIndex = null,
            int? pageSize = null)
        {
            // Lấy tất cả dịch vụ thỏa mãn điều kiện khuyến mãi trước khi phân trang
            var allServices = await _serviceRepository.GetServicesIncludeBusinessRankAsync(name, status, null, null, OrderByEnum.IdAsc);

            // Lọc các dịch vụ có khuyến mãi và trạng thái khuyến mãi là "Available"
            var discountedServices = allServices?.Where(service =>
                (service.Promotion != null &&
                Util.IsEqual(service.Promotion.Status, Util.TranslateGeneralStatus(StatusConstants.AVAILABLE)))
            );

            // Tính tổng số lượng dịch vụ khuyến mãi
            var totalCount = discountedServices?.Count() ?? 0;

            // Áp dụng phân trang sau khi đã tính tổng số lượng
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10;
                discountedServices = discountedServices?.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            return (discountedServices, totalCount);
        }
        public async Task<(IEnumerable<Service>?, int)> GetServicesByBranchIdAsync(
            int branchId,
            string? keyword = null,
            string? status = null,
            int? pageIndex = null,
            int? pageSize = null,
            OrderByEnum orderBy = OrderByEnum.IdAsc)
        {
            // Chỉ lấy danh sách dịch vụ theo keyword và status từ repository
            var services = await _serviceRepository.GetServicesAsync(keyword, status, null, null, orderBy);

            // Lọc các dịch vụ theo branchId
            var filteredServices = services?.Where(s => s.BranchServices.Any(bs => bs.BranchId == branchId));

            // Đếm tổng số dịch vụ sau khi lọc
            int totalCount = filteredServices?.Count() ?? 0;

            // Sắp xếp các dịch vụ dựa trên giá trị của orderBy
            filteredServices = orderBy == OrderByEnum.IdAsc
                ? filteredServices?.OrderBy(s => s.Id)
                : filteredServices?.OrderByDescending(s => s.Id);

            // Thực hiện phân trang nếu pageIndex và pageSize có giá trị
            if (pageIndex.HasValue && pageSize.HasValue && pageSize > 0)
            {
                filteredServices = filteredServices?
                    .Skip((pageIndex.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            }

            return (filteredServices, totalCount);
        }

        public async Task<(IEnumerable<Service>?, int)> GetServicesByCategoryIdAsync(
            int categoryId,
            string? keyword = null,
            string? status = null,
            int? pageIndex = null,
            int? pageSize = null,
            OrderByEnum orderBy = OrderByEnum.IdAsc)
        {
            // Chỉ lấy danh sách dịch vụ theo keyword và status từ repository
            var services = await _serviceRepository.GetServicesAsync(keyword, status, null, null, orderBy);

            // Lọc các dịch vụ theo branchId
            var filteredServices = services?.Where(s => s.CategoryId == categoryId);

            // Đếm tổng số dịch vụ sau khi lọc
            int totalCount = filteredServices?.Count() ?? 0;

            // Sắp xếp các dịch vụ dựa trên giá trị của orderBy
            filteredServices = orderBy == OrderByEnum.IdAsc
                ? filteredServices?.OrderBy(s => s.Id)
                : filteredServices?.OrderByDescending(s => s.Id);

            // Thực hiện phân trang nếu pageIndex và pageSize có giá trị
            if (pageIndex.HasValue && pageSize.HasValue && pageSize > 0)
            {
                filteredServices = filteredServices?
                    .Skip((pageIndex.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            }

            return (filteredServices, totalCount);
        }

        public Task<int> GetTotalServiceCountAsync(string? keyword = null, string? status = null)
        {
            return _serviceRepository.GetTotalServiceCountAsync(keyword, status);
        }
        public async Task UpdateServiceAsync(Service service)
        {
            await _serviceRepository.UpdateServiceAsync(service);
        }
        public async Task<decimal> GetServiceFinalPriceAsync(int serviceId)
        {
            var service = await _serviceRepository.GetServiceByIdAsync(serviceId);
            if (service == null)
            {
                throw new KeyNotFoundException($"Dịch vụ với ID {serviceId} không tìm thấy.");
            }

            if (service.Promotion == null)
            {
                return service.Price;
            }

            var isPromotionActive = Util.IsEqual(service.Promotion.Status, StatusConstants.AVAILABLE);
            if (!isPromotionActive)
            {
                return service.Price;
            }

            decimal finalPrice = service.Promotion.NewPrice;

            return finalPrice;
        }
        public async Task<(IEnumerable<Service>?, int)> GetServicesByBusinessIdAsync(
            int businessId,
            string? keyword = null,
            string? status = null,
            int? pageIndex = null,
            int? pageSize = null,
            OrderByEnum orderBy = OrderByEnum.IdAsc)
        {
            // Chỉ lấy danh sách dịch vụ theo keyword và status từ repository
            var services = await _serviceRepository.GetServicesAsync(keyword, status, null, null, orderBy);

            // Lọc các dịch vụ theo branchId
            var filteredServices = services?.Where(s => s.BranchServices.Any(bs => bs.Branch.BusinessId == businessId));

            // Đếm tổng số dịch vụ sau khi lọc
            int totalCount = filteredServices?.Count() ?? 0;

            // Sắp xếp các dịch vụ dựa trên giá trị của orderBy
            filteredServices = orderBy == OrderByEnum.IdAsc
                ? filteredServices?.OrderBy(s => s.Id)
                : filteredServices?.OrderByDescending(s => s.Id);

            // Thực hiện phân trang nếu pageIndex và pageSize có giá trị
            if (pageIndex.HasValue && pageSize.HasValue && pageSize > 0)
            {
                filteredServices = filteredServices?
                    .Skip((pageIndex.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);
            }

            return (filteredServices, totalCount);
        }
    }
}