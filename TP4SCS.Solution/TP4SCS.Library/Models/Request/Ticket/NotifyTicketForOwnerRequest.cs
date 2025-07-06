using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.Ticket
{
    public class NotifyTicketForOwnerRequest
    {
        [Required]
        public int TicketId { get; set; }

        [Required]
        public int AccountId { get; set; }

        [Required]
        public int OrderId { get; set; }
    }
}
