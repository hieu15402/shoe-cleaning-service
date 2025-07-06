namespace TP4SCS.Library.Models.Request.Payment
{
    public class MoMoRequest
    {
        public int TransactionId { get; set; }

        public string Description { get; set; } = string.Empty;

        public long Balance { get; set; }
    }
}
