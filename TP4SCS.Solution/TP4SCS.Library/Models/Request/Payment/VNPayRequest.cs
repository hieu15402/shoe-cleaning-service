namespace TP4SCS.Library.Models.Request.Payment
{
    public class VnPayRequest
    {
        public int TransactionId { get; set; }

        public string Description { get; set; } = string.Empty;

        public double Balance { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
