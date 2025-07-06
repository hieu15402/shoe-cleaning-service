using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.Category
{
    public enum TicketCateStatus
    {
        AVAILABLE,
        UNAVAILABLE
    }

    public class UpdateTicketCategoryRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1, 5)]
        public int Priority { get; set; }

        [Required]
        public TicketCateStatus Status { get; set; }
    }
}
