using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TP4SCS.Library.Models.Request.General;

namespace TP4SCS.Library.Models.Request.Account
{
    public class GetEmployeeRequest
    {
        public string? SearchKey { get; set; }

        [DefaultValue(null)]
        public AccountSortOption? SortBy { get; set; }

        [DefaultValue(null)]
        public AccountStatus? Status { get; set; }

        [Required]
        [DefaultValue(null)]
        public int? BusinessId { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool IsDecsending { get; set; }

        [Required]
        [DefaultValue(10)]
        [Range(1, int.MaxValue)]
        public int PageSize { get; set; }


        [Required]
        [DefaultValue(1)]
        [Range(1, int.MaxValue)]
        public int PageNum { get; set; }
    }
}
