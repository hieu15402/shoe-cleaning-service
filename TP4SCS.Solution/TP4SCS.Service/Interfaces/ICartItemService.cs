using TP4SCS.Library.Models.Data;

namespace TP4SCS.Services.Interfaces
{
    public interface ICartItemService
    {
        Task AddItemToCartAsync(int userId, int serviceId, List<int>? materialIds, int branchId);
        Task<CartItem?> GetCartItemByIdAsync(int itemId);
        Task<IEnumerable<CartItem>?> GetCartItemsAsync(int cartId);
        Task RemoveItemsFromCartAsync(List<int> itemIds);
        Task<decimal> CalculateCartItemsTotal(List<int> cartItemIds);
    }
}
