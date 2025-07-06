using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.Account
{
    public class UpdateAccountRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(10, MinimumLength = 10)]
        [DefaultValue("string")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        public DateOnly? Dob { get; set; }

        [Required]
        public string ImageUrl { get; set; } = string.Empty;
    }
}
