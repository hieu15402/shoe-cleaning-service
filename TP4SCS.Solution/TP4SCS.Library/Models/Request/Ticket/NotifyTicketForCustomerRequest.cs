using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.Ticket
{
    public class NotifyTicketForCustomerRequest
    {
        [Required]
        public int AccountId { get; set; }

        [Required]
        public int TicketId { get; set; }
    }
}
