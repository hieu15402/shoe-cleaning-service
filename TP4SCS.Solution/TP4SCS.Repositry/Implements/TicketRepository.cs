using DocumentFormat.OpenXml.Bibliography;
using Microsoft.EntityFrameworkCore;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Ticket;
using TP4SCS.Library.Models.Response.AssetUrl;
using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.Ticket;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Repository.Interfaces;

namespace TP4SCS.Repository.Implements
{
    public class TicketRepository : GenericRepository<SupportTicket>, ITicketRepository
    {
        public TicketRepository(Tp4scsDevDatabaseContext dbConext) : base(dbConext)
        {
        }
        public async Task CreateTicketAsync(SupportTicket supportTicket)
        {
            await InsertAsync(supportTicket);
        }

        public async Task DeleteTicketAsync(int id)
        {
            await DeleteAsync(id);
        }

        public async Task<SupportTicket?> GetModeratorChildTicketByParentId(int id)
        {
            int[] ids = await _dbContext.Accounts
                .AsNoTracking()
                .Where(a => a.Role.Equals(RoleConstants.MODERATOR) &&
                    a.Status.Equals(StatusConstants.ACTIVE))
                .Select(a => a.Id)
                .ToArrayAsync();

            return await _dbContext.SupportTickets
                .AsNoTracking()
                .Where(t => ids.Contains(t.UserId) &&
                    t.ParentTicketId == id)
                .FirstOrDefaultAsync();
        }

        public async Task<TicketResponse?> GetTicketByIdAsync(int id)
        {
            var childTickets = await _dbContext.SupportTickets
                .Where(c => c.ParentTicketId == id)
                .Select(c => new TicketResponse
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    FullName = _dbContext.Accounts
                            .AsNoTracking()
                            .Where(a => a.Id == c.UserId)
                            .Select(a => a.FullName)
                            .FirstOrDefault()!,
                    CategoryId = c.CategoryId,
                    CategoryName = _dbContext.TicketCategories
                            .AsNoTracking()
                            .Where(cn => cn.Id == c.CategoryId)
                            .Select(c => c.Name)
                            .FirstOrDefault()!,
                    Title = c.Title,
                    Content = c.Content,
                    CreateTime = c.CreateTime,
                    Assets = _dbContext.AssetUrls
                            .AsNoTracking()
                            .Where(a => a.TicketId == c.Id)
                            .Select(a => new FileResponse
                            {
                                Url = a.Url,
                                Type = a.Type
                            })
                            .ToList(),
                })
                .OrderBy(c => c.CreateTime)
                .ToListAsync();

            return await _dbContext.SupportTickets
                .Where(t => t.Id == id)
                .Select(t => new TicketResponse
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    FullName = _dbContext.Accounts
                        .AsNoTracking()
                        .Where(a => a.Id == t.UserId)
                        .Select(a => a.FullName)
                        .FirstOrDefault()!,
                    ModeratorId = t.ModeratorId,
                    ModeratorName = _dbContext.Accounts
                        .AsNoTracking()
                        .Where(a => a.Id == t.ModeratorId)
                        .Select(a => a.FullName)
                        .FirstOrDefault()!,
                    CategoryId = t.CategoryId,
                    CategoryName = _dbContext.TicketCategories
                        .AsNoTracking()
                        .Where(c => c.Id == t.CategoryId)
                        .Select(c => c.Name)
                        .FirstOrDefault()!,
                    Priority = _dbContext.TicketCategories
                        .AsNoTracking()
                        .Where(c => c.Id == t.CategoryId)
                        .Select(c => c.Priority)
                        .FirstOrDefault()!,
                    OrderId = t.OrderId,
                    Title = t.Title,
                    Content = t.Content,
                    CreateTime = t.CreateTime,
                    Status = t.Status,
                    Assets = _dbContext.AssetUrls
                        .AsNoTracking()
                        .Where(a => a.TicketId == t.Id)
                        .Select(a => new FileResponse
                        {
                            Url = a.Url,
                            Type = a.Type
                        })
                        .ToList(),
                    ChildTicket = childTickets,

                })
                .FirstOrDefaultAsync();
        }

        public async Task<(IEnumerable<TicketsResponse>?, Pagination)> GetTicketsAsync(GetTicketRequest getTicketRequest)
        {
            var tickets = _dbContext.SupportTickets
                .AsNoTracking()
                .Where(t => t.IsParentTicket == true)
                .Select(t => new TicketsResponse
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    FullName = _dbContext.Accounts
                        .AsNoTracking()
                        .Where(a => a.Id == t.UserId)
                        .Select(a => a.FullName)
                        .FirstOrDefault()!,
                    ModeratorId = t.ModeratorId,
                    ModeratorName = _dbContext.Accounts
                        .AsNoTracking()
                        .Where(a => a.Id == t.ModeratorId)
                        .Select(a => a.FullName)
                        .FirstOrDefault()!,
                    CategoryId = t.CategoryId,
                    CategoryName = _dbContext.TicketCategories
                        .AsNoTracking()
                        .Where(c => c.Id == t.CategoryId)
                        .Select(c => c.Name)
                        .FirstOrDefault()!,
                    Priority = _dbContext.TicketCategories
                        .AsNoTracking()
                        .Where(c => c.Id == t.CategoryId)
                        .Select(c => c.Priority)
                        .FirstOrDefault()!,
                    OrderId = t.OrderId,
                    Title = t.Title,
                    CreateTime = t.CreateTime,
                    IsSeen = t.IsSeen,
                    IsOwnerNoti = t.IsOwnerNoti,
                    Status = t.Status,
                })
                .OrderByDescending(c => c.Status.Equals(StatusConstants.OPENING))
                .ThenByDescending(c => c.IsSeen == false)
                .ThenByDescending(c => c.Status.Equals(StatusConstants.PROCESSING))
                .ThenByDescending(c => c.Status.Equals(StatusConstants.RESOLVING))
                .ThenByDescending(c => c.Status.Equals(StatusConstants.CLOSED))
                .ThenByDescending(c => c.Status.Equals(StatusConstants.CANCELED))
                .ThenByDescending(c => c.Priority)
                .ThenBy(c => c.CreateTime)
                .AsQueryable();

            //Search
            if (!string.IsNullOrEmpty(getTicketRequest.SearchKey))
            {
                string searchKey = getTicketRequest.SearchKey;
                tickets = tickets.Where(a => EF.Functions.Like(a.FullName, $"%{searchKey}%") ||
                    EF.Functions.Like(a.Title, $"%{searchKey}%") ||
                    EF.Functions.Like(a.CategoryName, $"%{searchKey}%"));
            }

            //Account Sort
            if (getTicketRequest.AccountId.HasValue)
            {
                tickets = tickets.Where(t => t.UserId == getTicketRequest.AccountId);
            }

            //Status Sort
            if (getTicketRequest.Status.HasValue)
            {
                tickets = getTicketRequest.Status switch
                {
                    TicketStatus.OPENING => tickets.Where(a => a.Status.Equals(StatusConstants.OPENING)),
                    TicketStatus.PROCESSING => tickets.Where(a => a.Status.Equals(StatusConstants.PROCESSING)),
                    TicketStatus.CLOSED => tickets.Where(a => a.Status.Equals(StatusConstants.CLOSED)),
                    TicketStatus.CANCELED => tickets.Where(a => a.Status.Equals(StatusConstants.CANCELED)),
                    _ => tickets
                };
            }

            //Order Sort
            if (getTicketRequest.SortBy.HasValue)
            {
                tickets = getTicketRequest.SortBy switch
                {
                    TicketSortOption.FULLNAME => getTicketRequest.IsDecsending
                                ? tickets.OrderByDescending(a => a.FullName)
                                : tickets.OrderBy(a => a.FullName),
                    TicketSortOption.CATEGORY => getTicketRequest.IsDecsending
                                  ? tickets.OrderByDescending(a => a.CategoryName)
                                  : tickets.OrderBy(a => a.CategoryName),
                    TicketSortOption.TITLE => getTicketRequest.IsDecsending
                                  ? tickets.OrderByDescending(a => a.Title)
                                  : tickets.OrderBy(a => a.Title),
                    TicketSortOption.PRIORITY => getTicketRequest.IsDecsending
                                  ? tickets.OrderByDescending(a => a.Priority)
                                  : tickets.OrderBy(a => a.Priority),
                    TicketSortOption.STATUS => getTicketRequest.IsDecsending
                                  ? tickets.OrderByDescending(a => a.Status)
                                  : tickets.OrderBy(a => a.Status),
                    _ => tickets
                };
            }

            //Count Total Data
            int totalData = await tickets.AsNoTracking().CountAsync();

            //Paging
            int skipNum = (getTicketRequest.PageNum - 1) * getTicketRequest.PageSize;
            tickets = tickets.Skip(skipNum).Take(getTicketRequest.PageSize);

            var result = await tickets.ToListAsync();

            int totalPage = (int)Math.Ceiling((decimal)totalData / getTicketRequest.PageSize);

            var pagination = new Pagination(totalData, getTicketRequest.PageSize, getTicketRequest.PageNum, totalPage);

            return (result, pagination);
        }

        public async Task<(IEnumerable<TicketsResponse>?, Pagination)> GetTicketsByBranchIdAsync(int id, GetBusinessTicketRequest getBusinessTicketRequest)
        {
            var tickets = _dbContext.SupportTickets
                .Include(t => t.Order)
                .ThenInclude(o => o!.OrderDetails)
                .ThenInclude(od => od.Service)
                .ThenInclude(s => s!.BranchServices)
                .Where(t => t.IsParentTicket == true &&
                            !t.Status.Equals(StatusConstants.PENDING) &&
                            t.Order!.OrderDetails
                               .Any(od => od.Service!.BranchServices
                                   .Any(bs => bs.Branch.Id == id)))
                .Select(t => new TicketsResponse
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    FullName = _dbContext.Accounts
                        .AsNoTracking()
                        .Where(a => a.Id == t.UserId)
                        .Select(a => a.FullName)
                        .FirstOrDefault()!,
                    ModeratorId = t.ModeratorId,
                    ModeratorName = _dbContext.Accounts
                        .AsNoTracking()
                        .Where(a => a.Id == t.UserId)
                        .Select(a => a.FullName)
                        .FirstOrDefault()!,
                    CategoryId = t.CategoryId,
                    CategoryName = _dbContext.TicketCategories
                        .AsNoTracking()
                        .Where(c => c.Id == t.CategoryId)
                        .Select(c => c.Name)
                        .FirstOrDefault()!,
                    Priority = _dbContext.TicketCategories
                        .AsNoTracking()
                        .Where(c => c.Id == t.CategoryId)
                        .Select(c => c.Priority)
                        .FirstOrDefault()!,
                    OrderId = t.OrderId,
                    Title = t.Title,
                    CreateTime = t.CreateTime,
                    IsSeen = t.IsSeen,
                    IsOwnerNoti = t.IsOwnerNoti,
                    Status = t.Status
                })
                .OrderBy(c => c.Status.Equals(StatusConstants.OPENING) ? 1
                            : c.Status.Equals(StatusConstants.PROCESSING) ? 2
                            : c.Status.Equals(StatusConstants.CLOSED) ? 3
                            : 4)
                .ThenBy(c => c.CreateTime)
                .ThenByDescending(c => c.Priority)
                .AsQueryable();

            //Search
            if (!string.IsNullOrEmpty(getBusinessTicketRequest.SearchKey))
            {
                string searchKey = getBusinessTicketRequest.SearchKey;
                tickets = tickets.Where(a =>
                    EF.Functions.Like(a.FullName, $"%{searchKey}%") ||
                    EF.Functions.Like(a.Title, $"%{searchKey}%") ||
                    EF.Functions.Like(a.CategoryName, $"%{searchKey}%"));
            }

            //Status Sort
            if (getBusinessTicketRequest.Status.HasValue)
            {
                tickets = getBusinessTicketRequest.Status switch
                {
                    TicketStatus.OPENING => tickets.Where(a => a.Status.Equals(StatusConstants.OPENING)),
                    TicketStatus.PROCESSING => tickets.Where(a => a.Status.Equals(StatusConstants.PROCESSING)),
                    TicketStatus.CLOSED => tickets.Where(a => a.Status.Equals(StatusConstants.CLOSED)),
                    TicketStatus.CANCELED => tickets.Where(a => a.Status.Equals(StatusConstants.CANCELED)),
                    _ => tickets
                };
            }

            //Order Sort
            if (getBusinessTicketRequest.SortBy.HasValue)
            {
                tickets = getBusinessTicketRequest.SortBy switch
                {
                    TicketSortOption.FULLNAME => getBusinessTicketRequest.IsDecsending
                                ? tickets.OrderByDescending(a => a.FullName)
                                : tickets.OrderBy(a => a.FullName),
                    TicketSortOption.CATEGORY => getBusinessTicketRequest.IsDecsending
                                  ? tickets.OrderByDescending(a => a.CategoryName)
                                  : tickets.OrderBy(a => a.CategoryName),
                    TicketSortOption.TITLE => getBusinessTicketRequest.IsDecsending
                                  ? tickets.OrderByDescending(a => a.Title)
                                  : tickets.OrderBy(a => a.Title),
                    TicketSortOption.PRIORITY => getBusinessTicketRequest.IsDecsending
                                  ? tickets.OrderByDescending(a => a.Priority)
                                  : tickets.OrderBy(a => a.Priority),
                    TicketSortOption.STATUS => getBusinessTicketRequest.IsDecsending
                                  ? tickets.OrderByDescending(a => a.Status)
                                  : tickets.OrderBy(a => a.Status),
                    _ => tickets
                };
            }

            //Count Total Data
            int totalData = await tickets.AsNoTracking().CountAsync();

            //Paging
            int skipNum = (getBusinessTicketRequest.PageNum - 1) * getBusinessTicketRequest.PageSize;
            tickets = tickets.Skip(skipNum).Take(getBusinessTicketRequest.PageSize);

            var result = await tickets.ToListAsync();

            int totalPage = (int)Math.Ceiling((decimal)totalData / getBusinessTicketRequest.PageSize);

            var pagination = new Pagination(totalData, getBusinessTicketRequest.PageSize, getBusinessTicketRequest.PageNum, totalPage);

            return (result, pagination);
        }

        public async Task<(IEnumerable<TicketsResponse>?, Pagination)> GetTicketsByBusinessIdAsync(int id, GetBusinessTicketRequest getBusinessTicketRequest)
        {
            var tickets = _dbContext.SupportTickets
                .Include(t => t.Order)
                .ThenInclude(o => o!.OrderDetails)
                .ThenInclude(od => od.Service)
                .ThenInclude(s => s!.BranchServices)
                .ThenInclude(bs => bs.Branch)
                .ThenInclude(b => b.Business)
                .Where(t => t.IsParentTicket == true &&
                            !t.Status.Equals(StatusConstants.PENDING) &&
                            t.Order!.OrderDetails
                                .Any(od => od.Service!.BranchServices
                                    .Any(bs => bs.Branch.Business.Id == id)))
                .Select(t => new TicketsResponse
                {
                    Id = t.Id,
                    UserId = t.UserId,
                    FullName = _dbContext.Accounts
                        .AsNoTracking()
                        .Where(a => a.Id == t.UserId)
                        .Select(a => a.FullName)
                        .FirstOrDefault()!,
                    ModeratorId = t.ModeratorId,
                    ModeratorName = _dbContext.Accounts
                        .AsNoTracking()
                        .Where(a => a.Id == t.UserId)
                        .Select(a => a.FullName)
                        .FirstOrDefault()!,
                    CategoryId = t.CategoryId,
                    CategoryName = _dbContext.TicketCategories
                        .AsNoTracking()
                        .Where(c => c.Id == t.CategoryId)
                        .Select(c => c.Name)
                        .FirstOrDefault()!,
                    Priority = _dbContext.TicketCategories
                        .AsNoTracking()
                        .Where(c => c.Id == t.CategoryId)
                        .Select(c => c.Priority)
                        .FirstOrDefault()!,
                    OrderId = t.OrderId,
                    Title = t.Title,
                    CreateTime = t.CreateTime,
                    IsSeen = t.IsSeen,
                    IsOwnerNoti = t.IsOwnerNoti,
                    Status = t.Status
                })
                .OrderByDescending(c => c.IsOwnerNoti == true)
                .ThenByDescending(c => c.Status.Equals(StatusConstants.OPENING))
                .ThenByDescending(c => c.Status.Equals(StatusConstants.PROCESSING))
                .ThenByDescending(c => c.Status.Equals(StatusConstants.RESOLVING))
                .ThenByDescending(c => c.Status.Equals(StatusConstants.CLOSED))
                .ThenByDescending(c => c.Status.Equals(StatusConstants.CANCELED))
                .ThenByDescending(c => c.Priority)
                .ThenBy(c => c.CreateTime)
                .AsQueryable();

            //Search
            if (!string.IsNullOrEmpty(getBusinessTicketRequest.SearchKey))
            {
                string searchKey = getBusinessTicketRequest.SearchKey;
                tickets = tickets.Where(a =>
                    EF.Functions.Like(a.FullName, $"%{searchKey}%") ||
                    EF.Functions.Like(a.Title, $"%{searchKey}%") ||
                    EF.Functions.Like(a.CategoryName, $"%{searchKey}%"));
            }

            //Status Sort
            if (getBusinessTicketRequest.Status.HasValue)
            {
                tickets = getBusinessTicketRequest.Status switch
                {
                    TicketStatus.OPENING => tickets.Where(a => a.Status.Equals(StatusConstants.OPENING)),
                    TicketStatus.PROCESSING => tickets.Where(a => a.Status.Equals(StatusConstants.PROCESSING)),
                    TicketStatus.CLOSED => tickets.Where(a => a.Status.Equals(StatusConstants.CLOSED)),
                    TicketStatus.CANCELED => tickets.Where(a => a.Status.Equals(StatusConstants.CANCELED)),
                    _ => tickets
                };
            }

            //Order Sort
            if (getBusinessTicketRequest.SortBy.HasValue)
            {
                tickets = getBusinessTicketRequest.SortBy switch
                {
                    TicketSortOption.FULLNAME => getBusinessTicketRequest.IsDecsending
                                ? tickets.OrderByDescending(a => a.FullName)
                                : tickets.OrderBy(a => a.FullName),
                    TicketSortOption.CATEGORY => getBusinessTicketRequest.IsDecsending
                                  ? tickets.OrderByDescending(a => a.CategoryName)
                                  : tickets.OrderBy(a => a.CategoryName),
                    TicketSortOption.TITLE => getBusinessTicketRequest.IsDecsending
                                  ? tickets.OrderByDescending(a => a.Title)
                                  : tickets.OrderBy(a => a.Title),
                    TicketSortOption.PRIORITY => getBusinessTicketRequest.IsDecsending
                                  ? tickets.OrderByDescending(a => a.Priority)
                                  : tickets.OrderBy(a => a.Priority),
                    TicketSortOption.STATUS => getBusinessTicketRequest.IsDecsending
                                  ? tickets.OrderByDescending(a => a.Status)
                                  : tickets.OrderBy(a => a.Status),
                    _ => tickets
                };
            }

            //Count Total Data
            int totalData = await tickets.AsNoTracking().CountAsync();

            //Paging
            int skipNum = (getBusinessTicketRequest.PageNum - 1) * getBusinessTicketRequest.PageSize;
            tickets = tickets.Skip(skipNum).Take(getBusinessTicketRequest.PageSize);

            var result = await tickets.ToListAsync();

            int totalPage = (int)Math.Ceiling((decimal)totalData / getBusinessTicketRequest.PageSize);

            var pagination = new Pagination(totalData, getBusinessTicketRequest.PageSize, getBusinessTicketRequest.PageNum, totalPage);

            return (result, pagination);
        }

        public async Task<SupportTicket?> GetUpdateTicketByIdAsync(int id)
        {
            return await _dbContext.SupportTickets.SingleOrDefaultAsync(t => t.Id == id);
        }

        public async Task UpdateTicketAsync(SupportTicket supportTicket)
        {
            await UpdateAsync(supportTicket);
        }
    }
}
