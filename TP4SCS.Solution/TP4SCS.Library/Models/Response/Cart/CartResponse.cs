using TP4SCS.Library.Models.Response.CartItem;

namespace TP4SCS.Library.Models.Response.Cart
{
    public class CartResponse
    {
        public int Id { get; set; }

        public int AccountId { get; set; }

        public decimal TotalPrice { get; set; }

        public virtual ICollection<CartItemResponse>? CartItems { get; set; }
    }
}
