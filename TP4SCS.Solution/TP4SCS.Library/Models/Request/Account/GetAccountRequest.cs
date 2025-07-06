using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.General
{
    public enum AccountSortOption
    {
        EMAIL,
        FULLNAME,
        STATUS
    }

    public enum AccountStatus
    {
        ACTIVE,
        INACTIVE,
        SUSPENDED
    }

    public class GetAccountRequest
    {
        public string? SearchKey { get; set; }

        [DefaultValue(null)]
        public AccountSortOption? SortBy { get; set; }

        [DefaultValue(null)]
        public AccountStatus? Status { get; set; }

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
