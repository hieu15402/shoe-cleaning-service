namespace TP4SCS.Library.Models.Data;

public partial class Cart
{
    public int Id { get; set; }

    public int AccountId { get; set; }

    public decimal TotalPrice { get; set; }

    public virtual Account Account { get; set; } = null!;

    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
