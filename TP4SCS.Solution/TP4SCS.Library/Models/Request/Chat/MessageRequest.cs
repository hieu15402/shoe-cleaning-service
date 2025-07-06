using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.Chat
{
    public class MessageRequest
    {
        [Required]
        public string RoomId { get; set; } = string.Empty;

        [Required]
        public int SenderId { get; set; }

        public string? Content { get; set; }

        public List<string>? ImageUrls { get; set; }

        [Required]
        public bool IsImage { get; set; }
    }
}
