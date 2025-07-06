using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.Address
{
    public class UpdateAddressRequest
    {
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

    }
}
