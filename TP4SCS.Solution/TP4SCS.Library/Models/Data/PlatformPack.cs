namespace TP4SCS.Library.Models.Data;

public partial class PlatformPack
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public int Period { get; set; }

    public decimal Price { get; set; }

    public string? Feature { get; set; }

    public string Type { get; set; } = null!;

    public virtual ICollection<PackSubscription> PackSubscriptions { get; set; } = new List<PackSubscription>();
}
