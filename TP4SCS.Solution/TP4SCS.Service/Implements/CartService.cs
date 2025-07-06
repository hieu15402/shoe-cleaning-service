using TP4SCS.Library.Models.Data;
using TP4SCS.Library.Models.Request.Cart;
using TP4SCS.Library.Models.Request.CartItem;
using TP4SCS.Library.Models.Request.ShipFee;
using TP4SCS.Library.Models.Response.CartItem;
using TP4SCS.Library.Utils.StaticClass;
using TP4SCS.Library.Utils.Utils;
using TP4SCS.Repository.Interfaces;
using TP4SCS.Services.Interfaces;

namespace TP4SCS.Services.Implements
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IServiceService _serviceService;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IShipService _shipService;
        private readonly IAddressRepository _addressRepository;
        private readonly IBranchRepository _branchRepository;
        private readonly IBusinessRepository _businessRepository;
        private readonly IMaterialService _materialService;

        public CartService(ICartRepository cartRepository, IServiceService serviceService
            , ICartItemRepository cartItemRepository, IOrderRepository orderRepository,
            IShipService shipService,
            IAddressRepository addressRepository,
            IBranchRepository branchRepository,
            IMaterialService materialService,
            IBusinessRepository businessRepository)
        {
            _cartRepository = cartRepository;
            _serviceService = serviceService;
            _cartItemRepository = cartItemRepository;
            _orderRepository = orderRepository;
            _shipService = shipService;
            _addressRepository = addressRepository;
            _branchRepository = branchRepository;
            _materialService = materialService;
            _businessRepository = businessRepository;
        }

        public async Task ClearCartAsync(int cartId)
        {
            await _cartRepository.ClearCartAsync(cartId);
        }

        public async Task<Cart> CreateCartAsync(int userId)
        {
            return await _cartRepository.CreateCartAsync(userId);
        }

        public async Task<Cart?> GetCartByUserIdAsync(int userId)
        {
            return await _cartRepository.GetCartByUserIdAsync(userId);
        }

        public async Task<decimal> GetCartTotalAsync(int cartId)
        {
            Cart? cart = await _cartRepository.GetCartByIdAsync(cartId);
            decimal totalPrice = 0;

            if (cart == null || cart.CartItems == null || !cart.CartItems.Any())
            {
                return totalPrice;
            }

            foreach (var item in cart.CartItems)
            {
                if (item.ServiceId.HasValue)
                {
                    totalPrice += await _serviceService.GetServiceFinalPriceAsync(item.ServiceId!.Value);
                }
                if (!string.IsNullOrEmpty(item.MaterialIds))
                {
                    List<int> materialIds = Util.ConvertStringToList(item.MaterialIds);
                    foreach (var materialId in materialIds)
                    {
                        var material = await _materialService.GetMaterialByIdAsync(materialId);
                        totalPrice += material!.Price;
                    }
                }
            }
            return totalPrice;
        }
        public async Task CheckoutForServiceAsync(HttpClient httpClient, CheckoutForServiceRequest request)
        {
            List<Order> orders = new List<Order>();
            Order order = new Order
            {
                AccountId = request.AccountId,
                AddressId = request.AddressId,
                CreateTime = DateTime.Now,
                IsAutoReject = request.IsAutoReject,
                Status = StatusConstants.PENDING,
                ShippingUnit = request.IsShip ? "Giao Hàng Nhanh" : null,
                ShippingCode = request.IsShip ? "" : null,
                OrderDetails = new List<OrderDetail>()
            };

            decimal finalPrice = await _serviceService.GetServiceFinalPriceAsync(request.Item.ServiceId);
            var service = await _serviceService.GetServiceByIdAsync(request.Item.ServiceId);
            service!.OrderedNum++;
            await _serviceService.UpdateServiceAsync(service);

            if (request.Item.MaterialIds != null && request.Item.MaterialIds.Any())
            {
                foreach (var materialId in request.Item.MaterialIds)
                {
                    var material = await _materialService.GetMaterialByIdAsync(materialId);
                    finalPrice += material!.Price;
                    if (material.BranchMaterials.SingleOrDefault(bm => bm.BranchId == request.Item.BranchId)!.Storage == 0)
                    {
                        throw new ArgumentException($"Material Id : {materialId} ở Branch có Id : {request.Item.BranchId} đã hết hàng.");
                    }
                    material!.BranchMaterials.SingleOrDefault(bm => bm.BranchId == request.Item.BranchId)!.Storage--;
                    await _materialService.UpdateMaterialAsync(material);
                }
            }
            order.OrderDetails.Add(new OrderDetail
            {
                BranchId = request.Item.BranchId,
                ServiceId = request.Item.ServiceId,
                MaterialIds = (request.Item.MaterialIds != null && request.Item.MaterialIds.Any()) ? Util.ConvertListToString(request.Item.MaterialIds) : null,
                Note = request.Note,
                Price = finalPrice,
            });
            order.DeliveredFee = request.IsShip ? await GetFeeShip(httpClient, request.AddressId!.Value, request.Item.BranchId, 1) : 0;
            order.OrderPrice = finalPrice;
            order.PendingTime = DateTime.Now;
            order.CreateTime = DateTime.Now;
            order.TotalPrice = finalPrice + order.DeliveredFee;

            orders.Add(order);
            await _orderRepository.AddOrdersAsync(orders);
        }


        public async Task CheckoutForCartAsync(HttpClient httpClient, CheckoutCartRequest request)
        {
            var orders = new List<Order>();
            var cart = request.Cart;

            // Lấy các CartItems từ CartCheckout hiện tại
            var cartItems = await toCartItemForCheckoutResponse(request.Cart.CartItems);

            // Nhóm các CartItems theo BranchId
            var groupedItems = cartItems
                .GroupBy(item => item.CartItem.BranchId)
                .Select(group => new
                {
                    BranchId = group.Key,
                    Items = group.ToList()
                });

            foreach (var group in groupedItems)
            {
                var order = new Order
                {
                    AccountId = request.AccountId,
                    AddressId = request.AddressId,
                    CreateTime = DateTime.Now,
                    IsAutoReject = request.IsAutoReject,
                    Status = StatusConstants.PENDING,
                    ShippingUnit = cart.IsShip ? "Giao Hàng Nhanh" : null,
                    ShippingCode = cart.IsShip ? "" : null,
                    OrderDetails = new List<OrderDetail>()
                };

                decimal orderPrice = 0;
                int quantity = 0;

                foreach (var item in group.Items)
                {
                    decimal finalPrice = 0;
                    if (item.CartItem.ServiceId.HasValue)
                    {
                        finalPrice = await _serviceService.GetServiceFinalPriceAsync(item.CartItem.ServiceId.Value);
                        var service = await _serviceService.GetServiceByIdAsync(item.CartItem.ServiceId.Value);
                        service!.OrderedNum++;
                        await _serviceService.UpdateServiceAsync(service);
                    }
                    if (!string.IsNullOrEmpty(item.CartItem.MaterialIds))
                    {
                        var materialIds = Util.ConvertStringToList(item.CartItem.MaterialIds);
                        foreach (var materialId in materialIds)
                        {
                            var material = await _materialService.GetMaterialByIdAsync(materialId);
                            finalPrice += material!.Price;
                            var branchMaterial = material!.BranchMaterials.SingleOrDefault(bm => bm.BranchId == item.CartItem.BranchId);
                            if (branchMaterial == null)
                            {
                                throw new ArgumentException($"Material Id : {materialId} không tồn tại trong Branch có Id : {item.CartItem.BranchId}");
                            }
                            if (branchMaterial.Storage == 0)
                            {
                                throw new ArgumentException($"Material Id : {materialId} ở Branch có Id : {item.CartItem.BranchId} đã hết hàng.");
                            }
                            branchMaterial.Storage--;
                            await _materialService.UpdateMaterialAsync(material);
                        }
                    }
                    order.OrderDetails.Add(new OrderDetail
                    {
                        BranchId = item.CartItem.BranchId,
                        ServiceId = item.CartItem.ServiceId,
                        MaterialIds = item.CartItem.MaterialIds,
                        Note = item.Note,
                        Price = finalPrice,
                    });

                    orderPrice += finalPrice;
                    quantity++;
                }

                order.DeliveredFee = cart.IsShip ? await GetFeeShip(httpClient, request.AddressId!.Value, group.BranchId, quantity) : 0;
                order.OrderPrice = orderPrice;
                order.PendingTime = DateTime.Now;
                order.CreateTime = DateTime.Now;
                order.TotalPrice = orderPrice + order.DeliveredFee;
                order.Status = StatusConstants.PENDING;

                orders.Add(order);

                var branch = await _branchRepository.GetBranchByIdAsync(group.BranchId);
                if (branch == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy branch với id: {group.BranchId} ");
                }
                var business = await _businessRepository.GetBusinessProfileByIdAsync(branch.BusinessId);
                if (business == null)
                {
                    throw new KeyNotFoundException($"Không tìm thấy business với id: {branch.BusinessId}");
                }

                business.PendingAmount++;
                branch.PendingAmount++;

                await _businessRepository.UpdateBusinessProfileAsync(business);
                await _branchRepository.UpdateBranchAsync(branch);
            }

            await _orderRepository.AddOrdersAsync(orders);

            // Xóa các CartItem đã xử lý
            List<int> allCartItemIds = new List<int>();
            foreach (var item in request.Cart.CartItems)
            {
                allCartItemIds.Add(item.CartItemId);
            }
            if (allCartItemIds.Any())
            {
                await _cartItemRepository.RemoveItemsFromCartAsync(allCartItemIds);
            }
        }


        private async Task<List<CartItemForCheckoutResponse>> toCartItemForCheckoutResponse(List<CartItemForCheckoutRequest> request)
        {
            var response = new List<CartItemForCheckoutResponse>();
            foreach (var item in request)
            {
                var cartItem = await _cartItemRepository.GetCartItemByIdAsync(item.CartItemId);
                if (cartItem == null)
                {
                    throw new ArgumentException($"Không tìm thấy cart item với ID: {item.CartItemId}");
                }
                CartItemForCheckoutResponse itemResponse = new CartItemForCheckoutResponse
                {
                    CartItem = cartItem,
                    Note = item.Note
                };
                response.Add(itemResponse);
            }
            return response;
        }

        //public async Task CheckoutForCartAsyncV2(HttpClient httpClient, CheckoutCartRequest request)
        //{
        //    var orders = new List<Order>();

        //    foreach (var cart in request.Carts)
        //    {
        //        // Lấy các CartItems từ CartCheckout hiện tại
        //        var cartItems = await _cartItemRepository.GetCartItemsByIdsAsync(cart.CartItemIds);

        //        // Nhóm các CartItems theo BranchId
        //        var groupedItems = cartItems
        //            .GroupBy(item => item.BranchId)
        //            .Select(group => new
        //            {
        //                BranchId = group.Key,
        //                Items = group.ToList()
        //            });

        //        foreach (var group in groupedItems)
        //        {
        //            var order = new Order
        //            {
        //                AccountId = request.AccountId,
        //                AddressId = request.AddressId,
        //                CreateTime = DateTime.Now,
        //                IsAutoReject = request.IsAutoReject,
        //                //Note = cart.Note,
        //                Status = StatusConstants.PENDING,
        //                ShippingUnit = cart.IsShip ? "Giao Hàng Nhanh" : null,
        //                ShippingCode = cart.IsShip ? "" : null,
        //                OrderDetails = new List<OrderDetail>()
        //            };

        //            decimal orderPrice = 0;
        //            int quantity = 0;

        //            foreach (var item in group.Items)
        //            {
        //                decimal finalPrice = 0;

        //                // Kiểm tra nếu là Material (có cả ServiceId và MaterialId)
        //                if (item.ServiceId.HasValue && item.MaterialId.HasValue)
        //                {
        //                    // Tính giá dựa trên Material
        //                    var material = await _materialService.GetMaterialByIdAsync(item.MaterialId.Value);
        //                    if (material == null)
        //                    {
        //                        throw new InvalidOperationException("Không tìm thấy material.");
        //                    }
        //                    finalPrice = material.Price;

        //                    order.OrderDetails.Add(new OrderDetail
        //                    {
        //                        BranchId = item.BranchId,
        //                        ServiceId = item.ServiceId,
        //                        MaterialId = item.MaterialId, // Thêm MaterialId vào OrderDetail
        //                        //Quantity = item.Quantity,
        //                        Price = finalPrice,
        //                    });
        //                }
        //                // Kiểm tra nếu là Service (chỉ có ServiceId)
        //                else if (item.ServiceId.HasValue && !item.MaterialId.HasValue)
        //                {
        //                    // Tính giá dựa trên Service
        //                    finalPrice = await _serviceService.GetServiceFinalPriceAsync(item.ServiceId.Value);

        //                    order.OrderDetails.Add(new OrderDetail
        //                    {
        //                        BranchId = item.BranchId,
        //                        ServiceId = item.ServiceId,
        //                        MaterialId = null, // Không có MaterialId
        //                        //Quantity = item.Quantity,
        //                        Price = finalPrice,
        //                    });
        //                }
        //                else
        //                {
        //                    // Trường hợp không hợp lệ, ném exception
        //                    throw new InvalidOperationException("CartItem không hợp lệ: phải có ServiceId hoặc cả ServiceId và MaterialId.");
        //                }

        //                // Cập nhật tổng giá và tổng số lượng
        //                //orderPrice += finalPrice * item.Quantity;
        //                //quantity += item.Quantity;
        //            }

        //            order.DeliveredFee = cart.IsShip ? await GetFeeShip(httpClient, request.AddressId!.Value, group.BranchId, quantity) : 0;
        //            order.OrderPrice = orderPrice;
        //            order.PendingTime = DateTime.Now;
        //            order.TotalPrice = orderPrice + order.DeliveredFee;

        //            orders.Add(order);
        //            UpdateBranchStatisticRequest branch = new UpdateBranchStatisticRequest();
        //            branch.Type = OrderStatistic.PENDING;

        //            await _businessBranchService.UpdateBranchStatisticAsync(order.OrderDetails.FirstOrDefault()!.BranchId, branch);

        //            UpdateBusinessStatisticRequest business = new UpdateBusinessStatisticRequest();
        //            business.Type = OrderStatistic.PENDING;

        //            await _businessService.UpdateBusinessStatisticAsync((await _businessBranchService.GetBranchByIdAsync(order.OrderDetails.FirstOrDefault()!.BranchId)).Data!.BusinessId, business);
        //        }
        //    }

        //    await _orderRepository.AddOrdersAsync(orders);

        //    // Xóa các CartItem đã xử lý
        //    var allCartItemIds = request.Carts.SelectMany(c => c.CartItemIds).ToArray();
        //    if (allCartItemIds.Any())
        //    {
        //        await _cartItemRepository.RemoveItemsFromCartAsync(allCartItemIds);
        //    }
        //}


        public async Task<decimal> GetFeeShip(HttpClient httpClient, int addressId, int branchId, int quantity)
        {
            var address = await _addressRepository.GetByIDAsync(addressId);
            var branch = await _branchRepository.GetBranchByIdAsync(branchId);

            if (branch == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy chi nhánh với ID: {branchId}");
            }
            if (address == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy địa chỉ của tài khoản với ID: {addressId}");
            }

            var getFeeShipRequest = new GetShipFeeRequest
            {
                FromDistricId = branch.DistrictId,
                FromWardCode = branch.WardCode,
                ToDistricId = address.DistrictId,
                ToWardCode = address.WardCode,
            };

            // Tính phí ship cho 1 hộp
            var baseFeeShip = await _shipService.GetShippingFeeAsync(httpClient, getFeeShipRequest);

            // Tính phí ship theo số lượng
            decimal totalFeeShip;
            if (quantity <= 5)
            {
                totalFeeShip = baseFeeShip;
            }
            else if (quantity <= 10)
            {
                totalFeeShip = baseFeeShip * 1.2m; // tăng 20%
            }
            else
            {
                totalFeeShip = baseFeeShip * 1.5m; // tăng 50%
            }

            return totalFeeShip;
        }


    }
}
