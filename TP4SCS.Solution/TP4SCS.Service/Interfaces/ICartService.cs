using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Cart;

namespace TP4SCS.Services.Interfaces
{
    public interface ICartService
    {
        Task<Cart?> GetCartByUserIdAsync(int userId);
        Task<Cart> CreateCartAsync(int userId);
        Task ClearCartAsync(int cartId);
        Task<decimal> GetCartTotalAsync(int cartId);
        Task CheckoutForCartAsync(HttpClient httpClient, CheckoutCartRequest request);
        Task CheckoutForServiceAsync(HttpClient httpClient, CheckoutForServiceRequest request);
        Task<decimal> GetFeeShip(HttpClient httpClient, int addressId, int branchId, int quantity);
        //Task CheckoutForCartAsyncV2(HttpClient httpClient, CheckoutCartRequest request);
        //Task UpdateCartAsync(Cart cart, int existingCartId);
    }
}
