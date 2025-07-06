using Microsoft.EntityFrameworkCore;
using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Library.Utils.Utils;
using TP4SCS.Repository.Interfaces;

namespace TP4SCS.Repository.Implements
{
    public class CartItemRepository : GenericRepository<CartItem>, ICartItemRepository
    {


        public CartItemRepository(Tp4scsDevDatabaseContext dbContext) : base(dbContext)
        {

        }

        public async Task AddItemToCartAsync(int userId, CartItem item)
        {
            // Lấy giỏ hàng của người dùng từ DbContext
            var cart = await _dbContext.Carts
                .Include(c => c.CartItems) // Đảm bảo bao gồm các CartItems
                .FirstOrDefaultAsync(c => c.AccountId == userId);

            // Nếu giỏ hàng không tồn tại, tạo một giỏ hàng mới
            if (cart == null)
            {
                cart = new Cart { AccountId = userId };
                await _dbContext.Carts.AddAsync(cart);
            }

            if (item.ServiceId.HasValue)
            {
                var service = await _dbContext.Services
                    .AsNoTracking()
                    .Include(s => s.BranchServices)
                    .Include(s => s.Materials)
                    .SingleOrDefaultAsync(s => s.Id == item.ServiceId.Value);
                if (service == null)
                {
                    throw new InvalidOperationException($"Dịch vụ với ID {item.ServiceId} không tìm thấy.");
                }

                if (!string.IsNullOrEmpty(item.MaterialIds))
                {
                    List<int> ids = Util.ConvertStringToList(item.MaterialIds);
                    var notFoundIds = ids.Except(service.Materials.Select(m => m.Id)).ToList();
                    if (notFoundIds.Any())
                    {
                        throw new ArgumentException($"Các Material IDs : {string.Join(", ", notFoundIds)} không tồn tại trong Service có id : {item.ServiceId}");
                    }
                }

                if (service.Status.ToUpper() == StatusConstants.UNAVAILABLE)
                {
                    throw new InvalidOperationException($"Dịch vụ với ID {item.ServiceId} đã ngừng hoạt động.");
                }

                if (!service.BranchServices.Any(bs => bs.BranchId == item.BranchId))
                {
                    throw new InvalidOperationException($"Dịch vụ với ID {item.ServiceId} không được cung cấp tại chi nhánh với ID {item.BranchId}.");
                }

            }
            item.CartId = cart.Id;

            cart.CartItems.Add(item);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<CartItem>> GetCartItemsByIdsAsync(int[] cartItemIds)
        {
            return await _dbContext.CartItems
                .Where(item => cartItemIds.Contains(item.Id))
                .Include(item => item.Service)
                .ToListAsync();
        }
        public async Task<CartItem?> GetCartItemByIdAsync(int itemId)
        {
            return await GetByIDAsync(itemId);
        }

        public async Task<IEnumerable<CartItem>?> GetCartItemsAsync(int cartId)
        {
            return await GetAsync(
                filter: item => item.CartId == cartId,
                includeProperties: "Service"
            );
        }


        public async Task RemoveItemsFromCartAsync(List<int> itemIds)
        {
            // Tìm tất cả các mục có Id nằm trong danh sách itemIds
            var itemsToRemove = await _dbContext.CartItems
                .Where(item => itemIds.Contains(item.Id))
                .ToListAsync();

            if (itemsToRemove.Count > 0)
            {
                // Xóa các mục đã tìm thấy
                _dbContext.CartItems.RemoveRange(itemsToRemove);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
