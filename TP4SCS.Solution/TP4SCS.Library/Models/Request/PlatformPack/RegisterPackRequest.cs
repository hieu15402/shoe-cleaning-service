using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.SubscriptionPack
{
    public class RegisterPackRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1, 12)]
        public int Period { get; set; }

        [Required]
        public decimal Price { get; set; }
    }
}
