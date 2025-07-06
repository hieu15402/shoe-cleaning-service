using Microsoft.EntityFrameworkCore;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Response.BusinessProfile;
using TP4SCS.Library.Models.Response.Leaderboard;
using TP4SCS.Repository.Interfaces;

namespace TP4SCS.Repository.Implements
{
    public class LeaderboardRepository : GenericRepository<Leaderboard>, ILeaderboardRepository
    {
        public LeaderboardRepository(Tp4scsDevDatabaseContext dbContext) : base(dbContext)
        {
        }

        public async Task CreateLeaderboardAsync(Leaderboard leaderboard)
        {
            await InsertAsync(leaderboard);
        }

        public async Task DeleteLeaderboardAsync(int id)
        {
            await DeleteAsync(id);
        }

        public async Task<LeaderboardResponse?> GetLeaderboardByMonthAsync()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            var leaderboardQuery = await _dbContext.Leaderboards
                .AsNoTracking()
                .Where(l => l.Month == currentMonth && l.Year == currentYear && l.IsMonth == true)
                .Select(l => new
                {
                    Leaderboard = l,
                    BusinessIds = l.BusinessIds
                })
                .FirstOrDefaultAsync();

            if (leaderboardQuery == null) return null;

            var businessIds = leaderboardQuery.BusinessIds.Split(',').Select(int.Parse).ToArray();

            var businesses = await _dbContext.BusinessProfiles
                .AsNoTracking()
                .Where(b => businessIds.Contains(b.Id))
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
                    Status = b.Status
                })
                .ToListAsync();

            businesses = businesses.OrderBy(b => Array.IndexOf(businessIds, b.Id)).ToList();

            return new LeaderboardResponse
            {
                Id = leaderboardQuery.Leaderboard.Id,
                Month = leaderboardQuery.Leaderboard.Month,
                Year = leaderboardQuery.Leaderboard.Year,
                IsMonth = leaderboardQuery.Leaderboard.IsMonth,
                IsYear = leaderboardQuery.Leaderboard.IsYear,
                Businesses = businesses
            };
        }

        public async Task<Leaderboard?> GetLeaderboardByMonthNoTrackingAsync()
        {
            var currentMonth = DateTime.Now.Month;
            var currentYear = DateTime.Now.Year;

            return await _dbContext.Leaderboards
                .AsNoTracking()
                .Where(l => l.Month == currentMonth && l.Year == currentYear && l.IsMonth == true)
                .FirstOrDefaultAsync();
        }

        public async Task<LeaderboardResponse?> GetLeaderboardByYearAsync()
        {
            var currentYear = DateTime.Now.Year;

            var leaderboardQuery = await _dbContext.Leaderboards
                .AsNoTracking()
                .Where(l => l.Year == currentYear && l.IsYear == true)
                .Select(l => new
                {
                    Leaderboard = l,
                    BusinessIds = l.BusinessIds
                })
                .FirstOrDefaultAsync();

            if (leaderboardQuery == null) return null;

            var businessIds = leaderboardQuery.BusinessIds.Split(',').Select(int.Parse).ToArray();

            var businesses = await _dbContext.BusinessProfiles
                .AsNoTracking()
                .Where(b => businessIds.Contains(b.Id))
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
                    Status = b.Status
                })
                .ToListAsync();

            return new LeaderboardResponse
            {
                Id = leaderboardQuery.Leaderboard.Id,
                Month = leaderboardQuery.Leaderboard.Month,
                Year = leaderboardQuery.Leaderboard.Year,
                IsMonth = leaderboardQuery.Leaderboard.IsMonth,
                IsYear = leaderboardQuery.Leaderboard.IsYear,
                Businesses = businesses
            };
        }

        public async Task<Leaderboard?> GetLeaderboardByYearNoTrackingAsync()
        {
            var currentYear = DateTime.Now.Year;

            return await _dbContext.Leaderboards
                .AsNoTracking()
                .Where(l => l.Year == currentYear && l.IsYear == true)
                .FirstOrDefaultAsync();
        }
    }
}
