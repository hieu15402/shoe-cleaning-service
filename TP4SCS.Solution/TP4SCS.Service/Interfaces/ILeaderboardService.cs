using TP4SCS.Library.Models.Response.General;
using TP4SCS.Library.Models.Response.Leaderboard;

namespace TP4SCS.Services.Interfaces
{
    public interface ILeaderboardService
    {
        Task<ApiResponse<LeaderboardResponse?>> GetLeaderboardByMonthAsync();

        Task<ApiResponse<LeaderboardResponse?>> GetLeaderboardByYearAsync();

        Task<ApiResponse<LeaderboardResponse>> UpdateLeaderboardAsync();
    }
}
