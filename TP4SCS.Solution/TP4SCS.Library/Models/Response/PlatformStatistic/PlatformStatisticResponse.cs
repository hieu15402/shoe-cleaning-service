namespace TP4SCS.Library.Models.Response.PlatformStatistic
{
    public class PlatformStatisticResponse
    {
        public string Type { get; set; } = string.Empty;

        public List<PlatformStatisticValueResponse> Value { get; set; } = new List<PlatformStatisticValueResponse>();
    }
}
