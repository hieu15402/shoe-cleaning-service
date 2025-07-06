using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TP4SCS.Library.Models.Request.Branch
{
    public class UpdateBranchEmployeeRequest
    {
        [Required]
        public List<int> EmployeeIds { get; set; } = new List<int>();

        [Required]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }
}
