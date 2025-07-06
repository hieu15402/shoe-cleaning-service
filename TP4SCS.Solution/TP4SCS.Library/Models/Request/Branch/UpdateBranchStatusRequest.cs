using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.Branch
{
    public class UpdateBranchStatusRequest
    {
        [Required]
        public BranchStatus Status { get; set; }
    }
}
