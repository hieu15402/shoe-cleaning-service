using TP4SCS.Library.Models.Response.Branch;

namespace TP4SCS.Library.Models.Response.BranchMaterial
{
    public class BranchMaterialResponse
    {
        public BranchResponse Branch { get; set; } = null!;
        public string Status { get; set; } = null!;
        public int Storage { get; set; }
    }
}
