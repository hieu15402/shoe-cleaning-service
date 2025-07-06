namespace TP4SCS.Library.Models.Request.Business
{
    public enum OrderStatistic
    {
        PENDING,
        PROCESSING,
        FINISHED,
        CANCELED
    }

    public class UpdateBusinessStatisticRequest
    {
        public OrderStatistic Type { get; set; }
    }
}
