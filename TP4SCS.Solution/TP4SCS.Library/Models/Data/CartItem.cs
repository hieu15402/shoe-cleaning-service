namespace TP4SCS.Library.Models.Data;

public partial class CartItem
{
    public int Id { get; set; }

    public int CartId { get; set; }

    public int BranchId { get; set; }

    public int? ServiceId { get; set; }

    public string? MaterialIds { get; set; }

    public decimal Price { get; set; }

    public virtual BusinessBranch Branch { get; set; } = null!;

    public virtual Cart Cart { get; set; } = null!;

    public virtual Service? Service { get; set; }
}
