namespace TP4SCS.Library.Models.Data;

public partial class Promotion
{
    public int Id { get; set; }

    public int ServiceId { get; set; }

    public int SaleOff { get; set; }

    public decimal NewPrice { get; set; }

    public string Status { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
