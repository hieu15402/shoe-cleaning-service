namespace TP4SCS.Library.Models.Data;

public partial class OrderDetail
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int BranchId { get; set; }

    public int? ServiceId { get; set; }

    public string? MaterialIds { get; set; }

    public decimal Price { get; set; }

    public string? Note { get; set; }

    public string? ProcessState { get; set; }

    public virtual ICollection<AssetUrl> AssetUrls { get; set; } = new List<AssetUrl>();

    public virtual BusinessBranch Branch { get; set; } = null!;

    public virtual Feedback? Feedback { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Service? Service { get; set; }
}
