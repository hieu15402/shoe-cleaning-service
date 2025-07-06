namespace TP4SCS.Library.Models.Response.CartItem
{
    public class CartItemForCheckoutResponse
    {
        public Data.CartItem CartItem { get; set; } = null!;
        public string? Note { get; set; }
    }
}
