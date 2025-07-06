namespace TP4SCS.Library.Models.Data;

public partial class BranchMaterial
{
    public int Id { get; set; }

    public int MaterialId { get; set; }

    public int BranchId { get; set; }

    public int Storage { get; set; }

    public string Status { get; set; } = null!;

    public virtual BusinessBranch Branch { get; set; } = null!;

    public virtual Material Material { get; set; } = null!;
}
