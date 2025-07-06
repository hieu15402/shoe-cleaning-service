using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.Auth
{
    public class RefreshToken
    {
        [Required]
        public int AccountId { get; set; }

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime RefreshExpireTime { get; set; }
    }
}
