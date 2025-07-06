namespace TP4SCS.Library.Models.Data;

public partial class Material
{
    public int Id { get; set; }

    public int ServiceId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public bool IsDefault { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<AssetUrl> AssetUrls { get; set; } = new List<AssetUrl>();

    public virtual ICollection<BranchMaterial> BranchMaterials { get; set; } = new List<BranchMaterial>();

    public virtual Service Service { get; set; } = null!;
}
