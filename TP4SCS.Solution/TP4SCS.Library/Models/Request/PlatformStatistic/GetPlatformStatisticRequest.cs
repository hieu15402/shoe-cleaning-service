namespace TP4SCS.Library.Models.Request.PlatformStatistic
{
    public enum StatisticOption
    {
        ORDER,
        PROFIT,
    }

    public class GetPlatformStatisticRequest
    {
        public StatisticOption Type { get; set; }

        public bool IsMonth { get; set; }
    }
}
