using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.WordBlacklist
{
    public class UpdateWordBlacklistRequest
    {
        [Required]
        public string Word { get; set; } = string.Empty;

        public string? Note { get; set; }
    }
}
