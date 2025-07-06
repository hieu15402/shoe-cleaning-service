using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.Ticket
{
    public class GetBusinessTicketRequest
    {
        public string? SearchKey { get; set; }

        [DefaultValue(null)]
        public TicketSortOption? SortBy { get; set; }

        [DefaultValue(null)]
        public TicketStatus? Status { get; set; }

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
