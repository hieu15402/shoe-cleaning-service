using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.Address
{
    public class CreateAddressRequest
    {
        [Required]
        public int AccountId { get; set; }

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        [DefaultValue("0")]
        public string WardCode { get; set; } = string.Empty;

        [Required]
        public int DistrictId { get; set; }

        [Required]
        public int ProvinceId { get; set; }

        [Required]
        [DefaultValue(false)]
        public bool IsDefault { get; set; }
    }
}