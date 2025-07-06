namespace TP4SCS.Library.Models.Data;

public partial class BranchService
{
    public int Id { get; set; }

    public int ServiceId { get; set; }

    public int BranchId { get; set; }

    public string Status { get; set; } = null!;

    public virtual BusinessBranch Branch { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
