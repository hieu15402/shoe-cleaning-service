namespace TP4SCS.Library.Models.Data;

public partial class TicketCategory
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Priority { get; set; }

    public string Status { get; set; } = null!;

    public virtual ICollection<SupportTicket> SupportTickets { get; set; } = new List<SupportTicket>();
}
