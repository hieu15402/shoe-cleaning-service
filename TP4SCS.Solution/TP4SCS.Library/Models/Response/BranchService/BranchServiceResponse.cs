using TP4SCS.Library.Models.Response.Branch;

namespace TP4SCS.Library.Models.Response.BranchService
{
    public class BranchServiceResponse
    {
        public BranchResponse Branch { get; set; } = null!;
        public string Status { get; set; } = null!;
    }
}
