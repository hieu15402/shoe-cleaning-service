namespace TP4SCS.Library.Models.Data;

public partial class PackSubscription
{
    public int Id { get; set; }

    public int BusinessId { get; set; }

    public int PackId { get; set; }

    public DateTime SubscriptionTime { get; set; }

    public virtual BusinessProfile Business { get; set; } = null!;

    public virtual PlatformPack Pack { get; set; } = null!;
}
