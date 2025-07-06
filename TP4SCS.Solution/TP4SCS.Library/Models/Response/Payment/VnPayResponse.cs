namespace TP4SCS.Library.Models.Response.Payment
{
    public class VnPayResponse
    {
        public int TransactionId { get; set; }

        public string PaymentId { get; set; } = string.Empty;

        public string PaymentMethod { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string VnPayResponseCode { get; set; } = string.Empty;
    }
}
