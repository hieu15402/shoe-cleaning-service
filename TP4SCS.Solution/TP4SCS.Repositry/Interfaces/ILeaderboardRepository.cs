using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Response.Leaderboard;

namespace TP4SCS.Repository.Interfaces
{
    public interface ILeaderboardRepository : IGenericRepository<Leaderboard>
    {
        Task<LeaderboardResponse?> GetLeaderboardByMonthAsync();

        Task<Leaderboard?> GetLeaderboardByMonthNoTrackingAsync();

        Task<LeaderboardResponse?> GetLeaderboardByYearAsync();

        Task<Leaderboard?> GetLeaderboardByYearNoTrackingAsync();

        Task CreateLeaderboardAsync(Leaderboard leaderboard);

        Task DeleteLeaderboardAsync(int id);
    }
}
