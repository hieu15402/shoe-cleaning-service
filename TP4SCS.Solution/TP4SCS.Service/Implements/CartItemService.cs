using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Utils.Utils;
using TP4SCS.Repository.Interfaces;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class CartItemService : ICartItemService
    {
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IServiceService _serviceService;
        private readonly IMaterialService _materialService;

        public CartItemService(ICartItemRepository cartItemRepository, IServiceService serviceService, IMaterialService materialService)
        {
            _cartItemRepository = cartItemRepository;
            _serviceService = serviceService;
            _materialService = materialService;
        }
        public async Task AddItemToCartAsync(int userId, int serviceId, List<int>? materialIds, int branchId)
        {
            CartItem item = new CartItem
            {
                ServiceId = serviceId,
                BranchId = branchId,
                MaterialIds = (materialIds != null && materialIds.Any()) ? Util.ConvertListToString(materialIds) : null
            };
            await _cartItemRepository.AddItemToCartAsync(userId, item);
        }
        public async Task<CartItem?> GetCartItemByIdAsync(int itemId)
        {
            return await _cartItemRepository.GetCartItemByIdAsync(itemId);
        }

        public async Task<IEnumerable<CartItem>?> GetCartItemsAsync(int cartId)
        {
            return await _cartItemRepository.GetCartItemsAsync(cartId);
        }

        public async Task RemoveItemsFromCartAsync(List<int> itemIds)
        {
            await _cartItemRepository.RemoveItemsFromCartAsync(itemIds);
        }

        public async Task<decimal> CalculateCartItemsTotal(List<int> cartItemIds)
        {
            decimal totalPrice = 0;
            var cartItems = new List<CartItem>();
            foreach (var id in cartItemIds)
            {
                CartItem? cartItem = await _cartItemRepository.GetCartItemByIdAsync(id);
                if (cartItem == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy mặt hàng giỏ hàng với ID {id}.");
                }
                cartItems.Add(cartItem);
            }

            foreach (var cartItem in cartItems)
            {
                decimal servicePrice = await _serviceService.GetServiceFinalPriceAsync(cartItem.ServiceId!.Value);

                if (servicePrice < 0 && cartItem.ServiceId.HasValue)
                {
                    throw new InvalidOperationException($"Giá dịch vụ không hợp lệ cho serviceId {cartItem.ServiceId.Value}.");
                }

                //totalPrice += servicePrice * cartItem.Quantity;
            }

            return totalPrice;
        }

    }
}
