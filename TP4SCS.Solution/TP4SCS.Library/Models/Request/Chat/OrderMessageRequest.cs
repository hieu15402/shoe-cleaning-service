using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.Chat
{
    public class OrderMessageRequest
    {
        [Required]
        public string RoomId { get; set; } = string.Empty;

        [Required]
        public int SenderId { get; set; }

        [Required]
        public int OrderId { get; set; }
    }
}
