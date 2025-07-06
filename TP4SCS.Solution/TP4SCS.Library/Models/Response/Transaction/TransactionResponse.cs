namespace TP4SCS.Library.Models.Response.Transaction
{
    public class TransactionResponse
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public string AccountName { get; set; } = string.Empty;

        public string PackName { get; set; } = string.Empty;

        public decimal Balance { get; set; }

        public DateTime ProcessTime { get; set; }

        public string? Description { get; set; }

        public string PaymentMethod { get; set; } = string.Empty;

        public string Status { get; set; } = string.Empty;
    }
}
