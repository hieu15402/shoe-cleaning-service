using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.Auth
{
    public class AccountRegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [DefaultValue("string")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(10, MinimumLength = 10)]
        [DefaultValue("string")]
        public string Phone { get; set; } = string.Empty;

        [Required]
        [DefaultValue("MALE")]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public DateOnly Dob { get; set; }
    }
}
