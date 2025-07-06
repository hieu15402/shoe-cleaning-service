namespace TP4SCS.Library.Models.Data;

public partial class AssetUrl
{
    public int Id { get; set; }

    public int? BusinessId { get; set; }

    public int? FeedbackId { get; set; }

    public int? MaterialId { get; set; }

    public int? ServiceId { get; set; }

    public int? OrderDetailId { get; set; }

    public int? TicketId { get; set; }

    public string Url { get; set; } = null!;

    public string Type { get; set; } = null!;

    public virtual BusinessProfile? Business { get; set; }

    public virtual Feedback? Feedback { get; set; }

    public virtual Material? Material { get; set; }

    public virtual OrderDetail? OrderDetail { get; set; }

    public virtual Service? Service { get; set; }

    public virtual SupportTicket? Ticket { get; set; }
}
