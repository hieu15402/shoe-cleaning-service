using System.ComponentModel.DataAnnotations;
using TP4SCS.Library.Models.Request.AssetUrl;

namespace TP4SCS.Library.Models.Request.Ticket
{
    public class CreateTicketRequest
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Content { get; set; } = string.Empty;

        public List<AssetUrlRequest>? Assets { get; set; }
    }
}
