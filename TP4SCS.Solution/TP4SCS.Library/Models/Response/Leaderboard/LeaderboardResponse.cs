using TP4SCS.Library.Models.Response.BusinessProfile;

namespace TP4SCS.Library.Models.Response.Leaderboard
{
    public class LeaderboardResponse
    {
        public int Id { get; set; }

        public int Month { get; set; }

        public int Year { get; set; }

        public bool IsMonth { get; set; }

        public bool IsYear { get; set; }

        public List<BusinessResponse> Businesses { get; set; } = new List<BusinessResponse>();
    }
}
