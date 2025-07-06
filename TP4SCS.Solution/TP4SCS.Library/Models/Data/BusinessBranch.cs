namespace TP4SCS.Library.Models.Data;

public partial class BusinessBranch
{
    public int Id { get; set; }

    public int BusinessId { get; set; }

    public string Name { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string WardCode { get; set; } = null!;

    public string Ward { get; set; } = null!;

    public int DistrictId { get; set; }

    public string District { get; set; } = null!;

    public int ProvinceId { get; set; }

    public string Province { get; set; } = null!;

    public string? EmployeeIds { get; set; }

    public int PendingAmount { get; set; }

    public int ProcessingAmount { get; set; }

    public int FinishedAmount { get; set; }

    public int CanceledAmount { get; set; }

    public bool IsDeliverySupport { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<BranchMaterial> BranchMaterials { get; set; } = new List<BranchMaterial>();

    public virtual ICollection<BranchService> BranchServices { get; set; } = new List<BranchService>();

    public virtual BusinessProfile Business { get; set; } = null!;

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
}
