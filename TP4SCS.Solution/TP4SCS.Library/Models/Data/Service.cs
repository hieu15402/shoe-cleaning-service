namespace TP4SCS.Library.Models.Data;

public partial class Service
{
    public int Id { get; set; }

    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    public decimal Rating { get; set; }

    public DateTime CreateTime { get; set; }

    public int OrderedNum { get; set; }

    public int FeedbackedNum { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<AssetUrl> AssetUrls { get; set; } = new List<AssetUrl>();

    public virtual ICollection<BranchService> BranchServices { get; set; } = new List<BranchService>();

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ServiceCategory Category { get; set; } = null!;

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual Promotion? Promotion { get; set; }

    public virtual ICollection<ServiceProcess> ServiceProcesses { get; set; } = new List<ServiceProcess>();
}
