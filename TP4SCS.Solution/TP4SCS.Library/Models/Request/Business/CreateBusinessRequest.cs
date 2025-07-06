using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.BusinessProfile
{
    public class CreateBusinessRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Phone]
        [StringLength(10, MinimumLength = 10)]
        [DefaultValue("string")]
        public string BusinessPhone { get; set; } = string.Empty;
    }
}
