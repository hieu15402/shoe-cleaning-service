namespace TP4SCS.Library.Models.Response.ShipOrder
{
    public class OrderLog
    {
        public string Status { get; set; } = string.Empty;
        public int PaymentTypeId { get; set; }
        public string TripCode { get; set; } = string.Empty;
        public string UpdatedDate { get; set; } = string.Empty;
    }
}
