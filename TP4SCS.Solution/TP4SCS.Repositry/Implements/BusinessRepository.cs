using Microsoft.EntityFrameworkCore;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Business;
using TP4SCS.Library.Models.Response.BusinessProfile;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.PackSubscription;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Repository.Interfaces;

namespace TP4SCS.Repository.Implements
{
    public class BusinessRepository : GenericRepository<BusinessProfile>, IBusinessRepository
    {
        private readonly IBranchRepository _branchRepository;

        public BusinessRepository(Tp4scsDevDatabaseContext dbContext, IBranchRepository branchRepository) : base(dbContext)
        {
            _branchRepository = branchRepository;
        }

        public async Task CreateBusinessProfileAsync(BusinessProfile businessProfile)
        {
            await InsertAsync(businessProfile);
        }

        public async Task<(IEnumerable<BusinessProfile>?, Pagination)> GetBusinessesProfilesAsync(GetBusinessRequest getBusinessRequest)
        {
            var businesses = _dbContext.BusinessProfiles.AsQueryable();

            //Search
            if (!string.IsNullOrEmpty(getBusinessRequest.SearchKey))
            {
                string searchKey = getBusinessRequest.SearchKey;
                businesses = businesses.Where(b => EF.Functions.Like(b.Name, $"%{searchKey}%"));
            }

            //Status Filter
            if (getBusinessRequest.Status != null)
            {
                //businesses = businesses.Where(b => b.Status.Equals(getBusinessRequest.Status));
                businesses = getBusinessRequest.Status switch
                {
                    BusinessStatus.ACTIVE => businesses.Where(b => b.Status.Equals(StatusConstants.ACTIVE)),
                    BusinessStatus.INACTIVE => businesses.Where(b => b.Status.Equals(StatusConstants.INACTIVE)),
                    BusinessStatus.SUSPENDED => businesses.Where(b => b.Status.Equals(StatusConstants.SUSPENDED)),
                    _ => businesses
                };
            }

            //Order Sort
            if (getBusinessRequest.SortBy != null)
            {
                businesses = getBusinessRequest.SortBy switch
                {
                    BusinessSortOption.NAME => getBusinessRequest.IsDecsending
                                ? businesses.OrderByDescending(b => b.Name)
                                : businesses.OrderBy(b => b.Name),
                    BusinessSortOption.RATING => getBusinessRequest.IsDecsending
                                ? businesses.OrderByDescending(b => b.Rating)
                                : businesses.OrderBy(b => b.Rating),
                    BusinessSortOption.TOTAL => getBusinessRequest.IsDecsending
                                ? businesses.OrderByDescending(b => b.TotalOrder)
                                : businesses.OrderBy(b => b.TotalOrder),
                    BusinessSortOption.PENDING => getBusinessRequest.IsDecsending
                                ? businesses.OrderByDescending(b => b.PendingAmount)
                                : businesses.OrderBy(b => b.PendingAmount),
                    BusinessSortOption.PROCESSING => getBusinessRequest.IsDecsending
                                ? businesses.OrderByDescending(b => b.ProcessingAmount)
                                : businesses.OrderBy(b => b.ProcessingAmount),
                    BusinessSortOption.FINISHED => getBusinessRequest.IsDecsending
                                ? businesses.OrderByDescending(b => b.FinishedAmount)
                                : businesses.OrderBy(b => b.FinishedAmount),
                    BusinessSortOption.CANCEL => getBusinessRequest.IsDecsending
                                ? businesses.OrderByDescending(b => b.CanceledAmount)
                                : businesses.OrderBy(b => b.CanceledAmount),
                    BusinessSortOption.STATUS => getBusinessRequest.IsDecsending
                                ? businesses.OrderByDescending(b => b.Status)
                                : businesses.OrderBy(b => b.Status),
                    _ => businesses
                };
            }

            //Count Total Data
            int totalData = await businesses.AsNoTracking().CountAsync();

            //Paging
            int skipNum = (getBusinessRequest.PageNum - 1) * getBusinessRequest.PageSize;
            businesses = businesses.Skip(skipNum).Take(getBusinessRequest.PageSize);

            //Paging Data Calulation
            var result = await businesses.ToListAsync();
            int totalPage = (int)Math.Ceiling((decimal)totalData / getBusinessRequest.PageSize);

            var paging = new Pagination(totalData, getBusinessRequest.PageSize, getBusinessRequest.PageNum, totalPage);

            return (result, paging);
        }
        public async Task<int> GetBusinessIdByOrderItemId(int id)
        {
            return await _dbContext.OrderDetails
                .Include(od => od.Branch)
                .Where(od => od.Id == id)
                .Select(od => od.Branch.BusinessId)
                .SingleOrDefaultAsync();
        }
        public async Task<int?> GetBusinessIdByOwnerIdAsync(int id)
        {
            return await _dbContext.BusinessProfiles
                .AsNoTracking()
                .Where(p => p.OwnerId == id)
                .Select(p => p.Id)
                .FirstOrDefaultAsync();
        }

        public async Task<BusinessProfile?> GetBusinessByOwnerIdAsync(int id)
        {
            return await _dbContext.BusinessProfiles.SingleOrDefaultAsync(p => p.OwnerId == id);
        }

        public async Task<BusinessProfile?> GetBusinessProfileByIdAsync(int id)
        {
            return await _dbContext.BusinessProfiles.SingleOrDefaultAsync(p => p.Id == id);
        }

        public async Task<int> GetBusinessProfileMaxIdAsync()
        {
            return await _dbContext.BusinessProfiles.AsNoTracking().MaxAsync(p => p.Id);
        }

        public async Task<bool> IsNameExistedAsync(string name)
        {
            return await _dbContext.BusinessProfiles
                .AsNoTracking()
                .AnyAsync(b => EF.Functions
                .Collate(b.Name, "SQL_Latin1_General_CP1_CI_AS")
                .Equals(name));
        }

        public async Task<bool> IsPhoneExistedAsync(string phone)
        {
            return await _dbContext.BusinessProfiles.AsNoTracking().AnyAsync(b => b.Phone.Equals(phone));
        }

        public async Task UpdateBusinessProfileAsync(BusinessProfile businessProfile)
        {
            await UpdateAsync(businessProfile);
        }

        public async Task<int> CountBusinessServiceByIdAsync(int id)
        {
            return await _dbContext.Services
                .AsNoTracking()
                //.GroupBy(s => new { s.Branch.BusinessId, s.Name })
                //.Select(s => new { s.Key.BusinessId, ServiceName = s.Key.Name })
                //.GroupBy(s => s.BusinessId)
                //.Select(s => new
                //{
                //    BusinessId = s.Key,
                //    DistinctService = s.ToList()
                //})
                .CountAsync();
        }

        public async Task<(IEnumerable<BusinessProfile>?, Pagination)> GetInvlaidateBusinessesProfilesAsync(GetInvalidateBusinessRequest getInvalidateBusinessRequest)
        {
            var businesses = _dbContext.BusinessProfiles.Where(b => b.Status.Equals(StatusConstants.PENDING)).AsQueryable();

            //Search
            if (!string.IsNullOrEmpty(getInvalidateBusinessRequest.SearchKey))
            {
                string searchKey = getInvalidateBusinessRequest.SearchKey;
                businesses = businesses.Where(b => EF.Functions.Like(b.Name, $"%{searchKey}%"));
            }

            //Order Sort
            if (getInvalidateBusinessRequest.SortBy != null)
            {
                businesses = getInvalidateBusinessRequest.IsDecsending
                                ? businesses.OrderByDescending(b => b.Name)
                                : businesses.OrderBy(b => b.Name);
            }

            //Count Total Data
            int totalData = await businesses.AsNoTracking().CountAsync();

            //Paging
            int skipNum = (getInvalidateBusinessRequest.PageNum - 1) * getInvalidateBusinessRequest.PageSize;
            businesses = businesses.Skip(skipNum).Take(getInvalidateBusinessRequest.PageSize);

            //Paging Data Calulation
            var result = await businesses.ToListAsync();
            int totalPage = (int)Math.Ceiling((decimal)totalData / getInvalidateBusinessRequest.PageSize);

            var paging = new Pagination(totalData, getInvalidateBusinessRequest.PageSize, getInvalidateBusinessRequest.PageNum, totalPage);

            if (result == null || !result.Any())
            {
                return (null, paging);
            }

            return (result, paging);
        }

        public async Task<int[]?> GetBusinessIdsAsync()
        {
            return await _dbContext.BusinessProfiles
                .AsNoTracking()
                .Where(b => b.Status.Equals(StatusConstants.ACTIVE))
                .Select(b => b.Id)
                .ToArrayAsync();
        }

        public async Task<BusinessResponse?> GetBusinessProfileByIdNoTrackingAsync(int id)
        {
            return await _dbContext.BusinessProfiles
                .AsNoTracking()
                .Where(b => b.Id == id)
                .Select(b => new BusinessResponse
                {
                    Id = b.Id,
                    OwnerId = b.OwnerId,
                    Name = b.Name,
                    Phone = b.Phone,
                    ImageUrl = b.ImageUrl,
                    Rating = b.Rating,
                    TotalOrder = b.TotalOrder,
                    PendingAmount = b.PendingAmount,
                    ProcessingAmount = b.ProcessingAmount,
                    FinishedAmount = b.FinishedAmount,
                    CanceledAmount = b.CanceledAmount,
                    ToTalServiceNum = b.ToTalServiceNum,
                    CreatedDate = b.CreatedDate,
                    RegisteredTime = b.RegisteredTime,
                    ExpiredTime = b.ExpiredTime,
                    IsIndividual = b.IsIndividual,
                    IsMaterialSupported = b.IsMaterialSupported,
                    IsLimitServiceNum = b.IsLimitServiceNum,
                    Status = b.Status,
                    PackSubscriptions = _dbContext.PackSubscriptions
                        .AsNoTracking()
                        .Where(s => s.BusinessId == b.Id)
                        .Select(s => new PackSubscriptionResponse
                        {
                            Id = s.Id,
                            PackId = s.PackId,
                            PackName = _dbContext.PlatformPacks
                                .AsNoTracking()
                                .Where(p => p.Id == s.PackId)
                                .Select(p => p.Name)
                                .SingleOrDefault()!,
                            SubscriptionTime = s.SubscriptionTime,
                        })
                        .ToList()
                })
                .SingleOrDefaultAsync();
        }

        public async Task<BusinessProfile?> GetBusinessIdByOwnerIdNoTrackingAsync(int id)
        {
            return await _dbContext.BusinessProfiles
                .AsNoTracking()
                .Where(b => b.OwnerId == id)
                .SingleOrDefaultAsync();
        }

        public async Task<BusinessProfile?> GetBusinessByOwnerIdNoTrackingAsync(int id)
        {
            return await _dbContext.BusinessProfiles
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.OwnerId == id);
        }

        public async Task<BusinessProfile?> GetBusinessByServiceIdAsync(int id)
        {
            int businessId = await _dbContext.Services
                .AsNoTracking()
                .Where(s => s.Id == id)
                .Include(s => s.BranchServices)
                    .ThenInclude(od => od.Branch)
                .Select(s => s.OrderDetails
                    .Select(od => od.Branch.BusinessId)
                    .FirstOrDefault())
                .SingleOrDefaultAsync();

            return await _dbContext.BusinessProfiles.SingleOrDefaultAsync(b => b.Id == businessId);
        }

        public async Task<BusinessProfile?> GetBusinessByOrderIdAsync(int id)
        {
            int businessId = await _dbContext.OrderDetails
                .AsNoTracking()
                .Where(o => o.OrderId == id)
                .Include(o => o.Branch)
                .Select(o => o.Branch.BusinessId)
                .FirstOrDefaultAsync();

            return await _dbContext.BusinessProfiles.SingleOrDefaultAsync(b => b.Id == businessId);
        }
    }
}