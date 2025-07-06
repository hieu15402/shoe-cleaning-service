namespace TP4SCS.Library.Models.Data;

public partial class Order
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public int? AddressId { get; set; }

    public DateTime CreateTime { get; set; }

    public DateTime? CanceledTime { get; set; }

    public DateTime? PendingTime { get; set; }

    public DateTime? ApprovedTime { get; set; }

    public DateTime? RevievedTime { get; set; }

    public DateTime? ProcessingTime { get; set; }

    public DateTime? StoragedTime { get; set; }

    public DateTime? ShippingTime { get; set; }

    public DateTime? DeliveredTime { get; set; }

    public DateTime? FinishedTime { get; set; }

    public DateTime? AbandonedTime { get; set; }

    public bool IsAutoReject { get; set; }

    public decimal OrderPrice { get; set; }

    public decimal DeliveredFee { get; set; }

    public decimal TotalPrice { get; set; }

    public string? ShippingUnit { get; set; }

    public string? ShippingCode { get; set; }

    public bool IsComplained { get; set; }

    public string Status { get; set; } = null!;

    public virtual Account Account { get; set; } = null!;

    public virtual AccountAddress? Address { get; set; }

    public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();

    public virtual ICollection<OrderNotification> OrderNotifications { get; set; } = new List<OrderNotification>();

    public virtual ICollection<SupportTicket> SupportTickets { get; set; } = new List<SupportTicket>();
}
