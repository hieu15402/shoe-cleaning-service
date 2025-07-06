using TP4SCS.Library.Models.Request.CartItem;

namespace TP4SCS.Library.Models.Request.Cart
{
    public class CartCheckout
    {
        public List<CartItemForCheckoutRequest> CartItems { get; set; } = new List<CartItemForCheckoutRequest>();
        public bool IsShip { get; set; } = false;
    }
}
