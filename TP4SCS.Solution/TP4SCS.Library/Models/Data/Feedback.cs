namespace TP4SCS.Library.Models.Data;

public partial class Feedback
{
    public int Id { get; set; }

    public int OrderItemId { get; set; }

    public decimal Rating { get; set; }

    public string? Content { get; set; }

    public string? Reply { get; set; }

    public DateTime CreatedTime { get; set; }

    public bool IsValidContent { get; set; }

    public bool IsValidAsset { get; set; }

    public bool IsAllowedUpdate { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<AssetUrl> AssetUrls { get; set; } = new List<AssetUrl>();

    public virtual OrderDetail OrderItem { get; set; } = null!;
}
