using TP4SCS.Library.Models.Data;

namespace TP4SCS.Repository.Interfaces
{
    public interface ICartItemRepository
    {
        Task AddItemToCartAsync(int cartId, CartItem item);
        Task RemoveItemsFromCartAsync(List<int> itemIds);
        Task<IEnumerable<CartItem>?> GetCartItemsAsync(int cartId);
        Task<CartItem?> GetCartItemByIdAsync(int itemId);
        Task<IEnumerable<CartItem>> GetCartItemsByIdsAsync(int[] cartItemIds);
    }
}
