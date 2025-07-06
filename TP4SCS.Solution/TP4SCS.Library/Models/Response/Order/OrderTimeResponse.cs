namespace TP4SCS.Library.Models.Response.Order
{
    public class OrderTimeResponse
    {
        public DateTime CreateTime { get; set; }

        public DateTime? CanceledTime { get; set; }

        public DateTime? PendingTime { get; set; }

        public DateTime? ApprovedTime { get; set; }

        public DateTime? RevievedTime { get; set; }

        public DateTime? ProcessingTime { get; set; }

        public DateTime? StoragedTime { get; set; }

        public DateTime? ShippingTime { get; set; }

        public DateTime? DeliveredTime { get; set; }

        public DateTime? FinishedTime { get; set; }

        public DateTime? AbandonedTime { get; set; }
    }
}
