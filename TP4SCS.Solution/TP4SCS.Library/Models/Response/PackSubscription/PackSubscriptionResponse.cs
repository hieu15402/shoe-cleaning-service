namespace TP4SCS.Library.Models.Response.PackSubscription
{
    public class PackSubscriptionResponse
    {
        public int Id { get; set; }

        public int PackId { get; set; }

        public string PackName { get; set; } = string.Empty;

        public DateTime SubscriptionTime { get; set; }
    }
}
