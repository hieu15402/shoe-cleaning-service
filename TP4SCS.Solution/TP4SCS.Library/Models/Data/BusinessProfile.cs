namespace TP4SCS.Library.Models.Data;

public partial class BusinessProfile
{
    public int Id { get; set; }

    public int OwnerId { get; set; }

    public string Name { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? ImageUrl { get; set; }

    public decimal Rating { get; set; }

    public int TotalOrder { get; set; }

    public int PendingAmount { get; set; }

    public int ProcessingAmount { get; set; }

    public int FinishedAmount { get; set; }

    public int CanceledAmount { get; set; }

    public int ToTalServiceNum { get; set; }

    public DateOnly CreatedDate { get; set; }

    public DateTime RegisteredTime { get; set; }

    public DateTime ExpiredTime { get; set; }

    public bool IsIndividual { get; set; }

    public bool IsMaterialSupported { get; set; }

    public bool IsLimitServiceNum { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<AssetUrl> AssetUrls { get; set; } = new List<AssetUrl>();

    public virtual ICollection<BusinessBranch> BusinessBranches { get; set; } = new List<BusinessBranch>();

    public virtual ICollection<BusinessStatistic> BusinessStatistics { get; set; } = new List<BusinessStatistic>();

    public virtual Account Owner { get; set; } = null!;

    public virtual ICollection<PackSubscription> PackSubscriptions { get; set; } = new List<PackSubscription>();
}
