namespace TP4SCS.Library.Models.Request.PlatformStatistic
{
    public enum UpdateStatisticOption
    {
        ORDER,
        PROFIT,
        USER
    }

    public class UpdatePlatformStatisticRequest
    {
        public UpdateStatisticOption Type { get; set; }
    }
}
