using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.General;
using TP4SCS.Library.Models.Response.Service;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Repository.Interfaces;

namespace TP4SCS.Repository.Implements
{
    public class ServiceRepository : GenericRepository<Service>, IServiceRepository
    {
        private readonly ILeaderboardRepository _leaderboardRepository;
        private readonly IMapper _mapper;
        public ServiceRepository(ILeaderboardRepository leaderboardRepository, IMapper mapper, Tp4scsDevDatabaseContext dbContext) : base(dbContext)
        {
            _leaderboardRepository = leaderboardRepository;
            _mapper = mapper;
        }

        public async Task AddServicesAsync(List<Service> services)
        {
            await _dbSet.AddRangeAsync(services);
            await _dbContext.SaveChangesAsync();
        }

        public async Task AddServiceAsync(int[] branchIds, int businessId, Service service)
        {
            var existingService = await _dbContext.Services
                            .AnyAsync(s => s.Name.ToLower() == service.Name.ToLower() && s.BranchServices.Any(bs => bs.Branch.BusinessId == businessId));

            if (existingService)
            {
                throw new InvalidOperationException($"Service with the name '{service.Name}' already exists for Business ID {businessId}.");
            }
            // Lấy tất cả các branch từ businessId
            var branches = await _dbContext.BusinessBranches
                               .Where(b => b.BusinessId == businessId)
                               .ToListAsync();

            // Thêm service mới vào cơ sở dữ liệu
            await _dbContext.Services.AddAsync(service);
            await _dbContext.SaveChangesAsync();

            // Kiểm tra trạng thái của service
            if (service.Status.ToLower() == StatusConstants.UNAVAILABLE.ToLower())
            {
                // Tạo BranchService cho mỗi branch với trạng thái UNAVAILABLE
                foreach (var branch in branches)
                {
                    var branchService = new BranchService
                    {
                        ServiceId = service.Id,
                        BranchId = branch.Id,
                        Status = StatusConstants.UNAVAILABLE.ToUpper()
                    };
                    await _dbContext.BranchServices.AddAsync(branchService);
                }

                // Lưu lại thay đổi vào cơ sở dữ liệu
                await _dbContext.SaveChangesAsync();
                return; // Kết thúc hàm nếu service là Unavailable
            }

            // Tạo BranchService cho mỗi branch và liên kết với service vừa tạo
            foreach (var branch in branches)
            {
                var branchService = new BranchService
                {
                    ServiceId = service.Id,
                    BranchId = branch.Id,
                    Status = StatusConstants.UNAVAILABLE.ToUpper()
                };

                if (branchIds.Contains(branch.Id) && service.Status.ToLower() == StatusConstants.AVAILABLE.ToLower())
                {
                    branchService.Status = StatusConstants.AVAILABLE.ToUpper();
                }

                await _dbContext.BranchServices.AddAsync(branchService);
            }

            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteServiceAsync(int id)
        {
            var branchServices = await _dbContext.BranchServices
                                .Where(bs => bs.ServiceId == id)
                                .ToListAsync();
            if (branchServices.Any())
            {
                _dbContext.BranchServices.RemoveRange(branchServices);
                await _dbContext.SaveChangesAsync();
            }
            await DeleteAsync(id);
        }

        public async Task<Service?> GetServiceByIdAsync(int id)
        {
            return await _dbContext.Services
                .Include(s => s.ServiceProcesses)
                .Include(s => s.Promotion)
                .Include(s => s.AssetUrls)
                .Include(s => s.BranchServices)
                .ThenInclude(bs => bs.Branch) // Bao gồm Branch trong BranchServices
                .Include(s => s.Category)
                .SingleOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Service>?> GetServicesAsync(
            string? keyword = null,
            string? status = null,
            int? pageIndex = null,
            int? pageSize = null,
            OrderByEnum orderBy = OrderByEnum.IdDesc)
        {
            // Xây dựng bộ lọc
            Expression<Func<Service, bool>> filter = s =>
                (string.IsNullOrEmpty(keyword) || s.Name.Contains(keyword)) &&
                (string.IsNullOrEmpty(status) || s.Status.ToLower().Trim() == status.ToLower().Trim());

            // Bắt đầu truy vấn với bộ lọc
            var query = _dbSet.Where(filter);

            // Bao gồm các thuộc tính liên quan
            query = query
                .Include(s => s.ServiceProcesses)
                .Include(s => s.Promotion)
                .Include(s => s.AssetUrls)
                .Include(s => s.Category)
                .Include(s => s.BranchServices) // Bao gồm BranchServices
                    .ThenInclude(bs => bs.Branch) // Bao gồm Branch trong BranchServices
                        .ThenInclude(b => b.Business); // Bao gồm Branch trong BranchServices
            query = orderBy switch
            {
                OrderByEnum.IdDesc => query.OrderByDescending(c => c.Id),
                _ => query.OrderBy(c => c.Id) // Mặc định sắp xếp theo Id tăng dần
            };
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                int validPageIndex = pageIndex > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize > 0 ? pageSize.Value : 10;

                query = query.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            // Trả về kết quả
            return await query.ToListAsync();
        }
        public async Task<IEnumerable<ServiceResponseV3>?> GetServicesIncludeBusinessRankAsync(
            string? keyword = null,
            string? status = null,
            int? pageIndex = null,
            int? pageSize = null,
            OrderByEnum orderBy = OrderByEnum.IdDesc)
        {
            // Xây dựng bộ lọc
            Expression<Func<Service, bool>> filter = s =>
                (string.IsNullOrEmpty(keyword) || s.Name.Contains(keyword)) &&
                (string.IsNullOrEmpty(status) || s.Status.ToLower().Trim() == status.ToLower().Trim());

            // Bắt đầu truy vấn với bộ lọc
            var query = _dbSet.Where(filter);

            // Lấy leaderboard từ repository
            var leaderShops = await _leaderboardRepository.GetLeaderboardByMonthAsync();

            // Lấy danh sách BusinessIds xếp hạng cao nhất
            var businessIds = leaderShops?.Businesses
                .Take(10) // Lấy tối đa 10 mục
                .Select(b => b.Id) // Lấy BusinessId
                .ToList();

            // Bao gồm các thuộc tính liên quan
            query = query
                .Include(s => s.ServiceProcesses)
                .Include(s => s.Promotion)
                .Include(s => s.AssetUrls)
                .Include(s => s.Category)
                .Include(s => s.BranchServices)
                    .ThenInclude(bs => bs.Branch)
                        .ThenInclude(b => b.Business);

            // Áp dụng sắp xếp
            query = orderBy switch
            {
                OrderByEnum.IdDesc => query.OrderByDescending(c => c.Id),
                _ => query.OrderBy(c => c.Id)
            };

            // Phân trang
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                int validPageIndex = pageIndex > 0 ? pageIndex.Value - 1 : 0;
                int validPageSize = pageSize > 0 ? pageSize.Value : 10;

                query = query.Skip(validPageIndex * validPageSize).Take(validPageSize);
            }

            // Lấy danh sách dịch vụ
            var services = await query.ToListAsync();

            // Sử dụng AutoMapper để chuyển đổi sang ServiceResponse
            var serviceResponses = _mapper.Map<List<ServiceResponseV3>>(services);

            // Gán BusinessRank cho từng ServiceResponse
            if (businessIds != null && businessIds.Any())
            {
                foreach (var serviceResponse in serviceResponses)
                {
                    var rank = businessIds.IndexOf(serviceResponse.BusinessId) + 1;
                    serviceResponse.BusinessRank = rank > 0 ? rank : null;
                }
            }

            return serviceResponses;
        }


        public async Task<IEnumerable<Service>> GetServicesAsync(string? keyword = null, string? status = null)
        {
            Expression<Func<Service, bool>> filter = s =>
                (string.IsNullOrEmpty(keyword) || s.Name.Contains(keyword)) &&
                (string.IsNullOrEmpty(status) || s.Status.ToLower() == status.ToLower());

            return await _dbContext.Services
                .AsNoTracking()
                .Include(s => s.Promotion)
                .Where(filter)
                .ToListAsync();
        }

        public async Task<int> GetTotalServiceCountAsync(string? keyword = null, string? status = null)
        {
            Expression<Func<Service, bool>> filter = s =>
                (string.IsNullOrEmpty(keyword) || s.Name.Contains(keyword)) &&
                (string.IsNullOrEmpty(status) || s.Status.ToLower() == status.ToLower());

            return await _dbContext.Services.AsNoTracking().CountAsync(filter);
        }

        public async Task UpdateServiceAsync(Service service, int[] branchIds)
        {
            var existingBranchServices = await _dbContext.BranchServices
                                .Where(bs => bs.ServiceId == service.Id)
                                .ToListAsync();
            if (service.Status.ToLower() == StatusConstants.UNAVAILABLE.ToLower())
            {
                // Cập nhật trạng thái của tất cả BranchService thành Unavailable
                foreach (var branchService in existingBranchServices)
                {
                    branchService.Status = StatusConstants.UNAVAILABLE.ToUpper();
                }
                // Lưu lại thay đổi vào cơ sở dữ liệu
                _dbContext.BranchServices.UpdateRange(existingBranchServices);
                await _dbContext.SaveChangesAsync();
                return; // Kết thúc hàm nếu service là Unavailable
            }


            foreach (var branchService in existingBranchServices)
            {
                // Kiểm tra xem BusinessId có tồn tại trong serviceUpdateRequest hay không
                if (branchIds.Contains(branchService.BranchId))
                {
                    branchService.Status = StatusConstants.AVAILABLE;
                }
                else
                {
                    branchService.Status = StatusConstants.UNAVAILABLE;
                }
            }

            await UpdateAsync(service);
        }

        public async Task UpdateServiceAsync(Service service)
        {
            await UpdateAsync(service);
        }

        public async Task<int> CountTotalServiceOfBusinessAsync(int id)
        {
            return await _dbContext.Services
                .AsNoTracking()
                .Include(s => s.BranchServices)
                    .ThenInclude(bs => bs.Branch)
                .Where(s => s.BranchServices.Any(bs => bs.Branch.BusinessId == id) &&
                    s.Status.Equals(StatusConstants.AVAILABLE))
                .CountAsync();
        }
    }
}
