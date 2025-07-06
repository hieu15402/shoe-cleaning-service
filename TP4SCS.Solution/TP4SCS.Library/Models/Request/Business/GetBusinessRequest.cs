using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.Business
{
    public enum BusinessSortOption
    {
        NAME,
        RATING,
        RANK,
        TOTAL,
        PENDING,
        PROCESSING,
        FINISHED,
        CANCEL,
        STATUS
    }

    public enum BusinessStatus
    {
        EXPRIED,
        ACTIVE,
        INACTIVE,
        SUSPENDED
    }

    public class GetBusinessRequest
    {
        public string? SearchKey { get; set; } = string.Empty;

        [DefaultValue(null)]
        public BusinessSortOption? SortBy { get; set; }

        [DefaultValue(null)]
        public BusinessStatus? Status { get; set; }

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
