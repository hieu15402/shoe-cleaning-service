using System.ComponentModel.DataAnnotations;
using TP4SCS.Library.Models.Request.Payment;

namespace TP4SCS.Library.Models.Request.Transaction
{
    public class UpdateTransactionRequest
    {
        [Required]
        public int PackId { get; set; }

        [Required]
        public decimal Balance { get; set; }

        [Required]
        public PaymentOptions PaymentMethod { get; set; }

        [Required]
        public string Status { get; set; } = string.Empty;
    }
}
