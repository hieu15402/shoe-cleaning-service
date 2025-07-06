namespace TP4SCS.Library.Models.Data;

public partial class OrderNotification
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public DateTime NotificationTime { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public bool IsProviderNoti { get; set; }

    public virtual Order Order { get; set; } = null!;
}
