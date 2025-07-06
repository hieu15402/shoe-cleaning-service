namespace TP4SCS.Library.Models.Data;

public partial class Transaction
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public string PackName { get; set; } = null!;

    public decimal Balance { get; set; }

    public DateTime ProcessTime { get; set; }

    public string? Description { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string Status { get; set; } = null!;

    public virtual Account Account { get; set; } = null!;
}
