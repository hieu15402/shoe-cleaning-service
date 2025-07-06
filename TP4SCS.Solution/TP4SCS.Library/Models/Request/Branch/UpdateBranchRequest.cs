using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.Branch
{
    public enum BranchStatus
    {
        ACTIVE,
        INACTIVE,
        SUSPENDED
    }

    public class UpdateBranchRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Address { get; set; } = string.Empty;

        [Required]
        [RegularExpression(@"^\d{1,9}$")]
        [DefaultValue("0")]
        public string WardCode { get; set; } = string.Empty;

        [Required]
        public int DistrictId { get; set; }

        [Required]
        public int ProvinceId { get; set; }

        [Required]
        [DefaultValue(true)]
        public bool IsDeliverySupport { get; set; }

        [Required]
        public BranchStatus Status { get; set; }
    }
}
