using System.ComponentModel.DataAnnotations;
using TP4SCS.Library.Models.Request.Payment;

namespace TP4SCS.Library.Models.Request.Transaction
{
    public class CreateTransactionRequest
    {
        [Required]
        public int AccountId { get; set; }

        [Required]
        public int PackId { get; set; }

        [Required]
        [Range(1000.00, double.MaxValue)]
        public decimal Balance { get; set; }

        [Required]
        public PaymentOptions PaymentMethod { get; set; }
    }
}
