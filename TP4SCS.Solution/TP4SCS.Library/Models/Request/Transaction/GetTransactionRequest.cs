using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.Transaction
{
    public enum TransactionSortOption
    {
        ACCOUNTNAME,
        PACKNAME,
        BALANCE,
        TIME,
        PAYMENT
    }

    public enum TransactionStatus
    {
        PENDING,
        EXPIRED,
        PROCESSING,
        COMPLETED,
        FAILED
    }

    public class GetTransactionRequest
    {
        [DefaultValue(null)]
        public int? AccountId { get; set; }

        [DefaultValue(null)]
        public string? SearchKey { get; set; }

        [DefaultValue(null)]
        public TransactionSortOption? SortBy { get; set; }

        [DefaultValue(null)]
        public TransactionStatus? Status { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool IsDecsending { get; set; }

        [Required]
        [DefaultValue(10)]
        [Range(1, int.MaxValue)]
        public int PageSize { get; set; } = 10;


        [Required]
        [DefaultValue(1)]
        [Range(1, int.MaxValue)]
        public int PageNum { get; set; }
    }
}
