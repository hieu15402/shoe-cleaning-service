using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.Service
{
    public class GetBusinessServiceRequest
    {
        [Required]
        public int BusinessId { get; set; }

        public string? SearchKey { get; set; }

        public string? SortBy { get; set; }

        public string? Category { get; set; }

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
