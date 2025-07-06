namespace TP4SCS.Library.Models.Data;

public partial class SupportTicket
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int? ModeratorId { get; set; }

    public int CategoryId { get; set; }

    public int? OrderId { get; set; }

    public int? ParentTicketId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public DateTime CreateTime { get; set; }

    public bool IsParentTicket { get; set; }

    public bool IsSeen { get; set; }

    public bool IsOwnerNoti { get; set; }

    public DateTime? AutoClosedTime { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<AssetUrl> AssetUrls { get; set; } = new List<AssetUrl>();

    public virtual TicketCategory Category { get; set; } = null!;

    public virtual Account? Moderator { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Account User { get; set; } = null!;
}
